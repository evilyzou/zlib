package zredis

import (
	"bufio"
	"bytes"
	"errors"
	"fmt"
	"io"
	"strconv"
)

// protocol's special bytes
const (
	cr_byte    byte = byte('\r')
	lf_byte         = byte('\n')
	space_byte      = byte(' ')
	err_byte        = byte('-')
	ok_byte         = byte('+')
	count_byte      = byte('*')
	size_byte       = byte('$')
	num_byte        = byte(':')
	true_byte       = byte('1')
)

type ctlbytes []byte

var crlf_bytes ctlbytes = ctlbytes{cr_byte, lf_byte}

func CreateRequestBytes(cmd *Command, args [][]byte) []byte {

	defer func() {
		if e := recover(); e != nil {
			panic(newSystemErrorf("CreateRequestBytes(%s) - failed to create request buffer", cmd.Code))
		}
	}()
	cmd_bytes := []byte(cmd.Code)

	buffer := bytes.NewBufferString("")
	buffer.WriteByte(count_byte)
	buffer.Write([]byte(strconv.Itoa(len(args) + 1)))
	buffer.Write(crlf_bytes)
	buffer.WriteByte(size_byte)
	buffer.Write([]byte(strconv.Itoa(len(cmd_bytes))))
	buffer.Write(crlf_bytes)
	buffer.Write(cmd_bytes)
	buffer.Write(crlf_bytes)

	for _, s := range args {
		buffer.WriteByte(size_byte)
		buffer.Write([]byte(strconv.Itoa(len(s))))
		buffer.Write(crlf_bytes)
		buffer.Write(s)
		buffer.Write(crlf_bytes)
	}

	return buffer.Bytes()
}

func sendRequest(w io.Writer, data []byte) {
	loginfo := "sendRequest"
	if w == nil {
		panic(newSystemErrorf("<BUG> %s() - nil Writer", loginfo))
	}

	n, e := w.Write(data)
	if e != nil {
		panic(newSystemErrorf("%s() - connection Write wrote %d bytes only.", loginfo, n))
	}

	if n < len(data) {
		panic(newSystemErrorf("%s() - connection Write wrote %d bytes only.", loginfo, n))
	}
}

// ----------------------------------------------------------------------------
// Response
// ----------------------------------------------------------------------------

type Response interface {
	IsError() bool
	GetMessage() string
	GetBooleanValue() bool
	GetNumberValue() int64
	GetStringValue() string
	GetBulkData() []byte
	GetMultiBulkData() [][]byte
}
type _response struct {
	isError       bool
	msg           string
	boolval       bool
	numval        int64
	stringval     string
	bulkdata      []byte
	multibulkdata [][]byte
}

func (r *_response) IsError() bool          { return r.isError }
func (r *_response) GetMessage() string     { return r.msg }
func (r *_response) GetBooleanValue() bool  { return r.boolval }
func (r *_response) GetNumberValue() int64  { return r.numval }
func (r *_response) GetStringValue() string { return r.stringval }
func (r *_response) GetBulkData() []byte    { return r.bulkdata }
func (r *_response) GetMultiBulkData() [][]byte {
	return r.multibulkdata
}

// ----------------------------------------------------------------------------
// response processing
// ----------------------------------------------------------------------------

// Gets the response to the command.
//
// The returned response (regardless of flavor) may have (application level)
// errors as sent from Redis server.  (Note err will be nil in that case)
//
// Any errors (whether runtime or bugs) are returned as redis.Error.
func GetResponse(reader *bufio.Reader, cmd *Command) (resp Response, err Error) {

	defer func() {
		err = onRecover(recover(), "GetResponse")
	}()

	buf := readToCRLF(reader)

	// Redis error
	if buf[0] == err_byte {
		resp = &_response{msg: string(buf[1:]), isError: true}
		return
	}

	switch cmd.RespType {
	case STATUS:
		resp = &_response{msg: string(buf[1:])}
		return
	case STRING:
		assertCtlByte(buf, ok_byte, "STRING")
		resp = &_response{stringval: string(buf[1:])}
		return
	case BOOLEAN:
		assertCtlByte(buf, num_byte, "BOOLEAN")
		resp = &_response{boolval: buf[1] == true_byte}
		return
	case NUMBER:
		assertCtlByte(buf, num_byte, "NUMBER")
		n, e := strconv.ParseInt(string(buf[1:]), 10, 64)
		assertNotError(e, "in GetResponse - parse error in NUMBER response")
		resp = &_response{numval: n}
		return
	case VIRTUAL:
		resp = &_response{boolval: true}
		return
	case BULK:
		assertCtlByte(buf, size_byte, "BULK")
		size, e := strconv.Atoi(string(buf[1:]))
		assertNotError(e, "in GetResponse - parse error in BULK size")
		resp = &_response{bulkdata: readBulkData(reader, size)}
		return
	case MULTI_BULK:
		assertCtlByte(buf, count_byte, "MULTI_BULK")
		cnt, e := strconv.Atoi(string(buf[1:]))
		assertNotError(e, "in GetResponse - parse error in MULTIBULK cnt")
		resp = &_response{multibulkdata: readMultiBulkData(reader, cnt)}
		return
	}

	panic(fmt.Errorf("BUG - GetResponse - this should not have been reached"))
}

// panics on error (with redis.Error)
func assertCtlByte(buf []byte, b byte, info string) {
	if buf[0] != b {
		panic(newSystemErrorf("control byte for %s is not '%s' as expected - got '%s'", info, string(b), string(buf[0])))
	}
}

// panics on error (with redis.Error)
func assertNotError(e error, info string) {
	if e != nil {
		panic(newSystemErrorWithCause(info, e))
	}
}

func readToCRLF(r *bufio.Reader) []byte {
	//	var buf []byte
	buf, e := r.ReadBytes(cr_byte)
	if e != nil {
		panic(newSystemErrorWithCause("readToCRLF - ReadBytes", e))
	}

	var b byte
	b, e = r.ReadByte()
	if e != nil {
		panic(newSystemErrorWithCause("readToCRLF - ReadByte", e))
	}
	if b != lf_byte {
		e = errors.New("<BUG> Expecting a Linefeed byte here!")
	}
	return buf[0 : len(buf)-1]
}

// Reads a multibulk response of given expected elements.
//
// panics on errors (with redis.Error)
func readBulkData(r *bufio.Reader, n int) (data []byte) {
	if n >= 0 {
		buffsize := n + 2
		data = make([]byte, buffsize)
		if _, e := io.ReadFull(r, data); e != nil {
			panic(newSystemErrorWithCause("readBulkData - ReadFull", e))
		} else {
			if data[n] != cr_byte || data[n+1] != lf_byte {
				panic(newSystemErrorf("terminal was not crlf_bytes as expected - data[n:n+1]:%s", data[n:n+1]))
			}
			data = data[:n]
		}
	}
	return
}

// Reads a multibulk response of given expected elements.
// The initial *num\r\n is assumed to have been consumed.
//
// panics on errors (with redis.Error)
func readMultiBulkData(conn *bufio.Reader, num int) [][]byte {
	data := make([][]byte, num)
	for i := 0; i < num; i++ {
		buf := readToCRLF(conn)
		if buf[0] != size_byte {
			panic(newSystemErrorf("readMultiBulkData - expected: size_byte got: %d", buf[0]))
		}

		size, e := strconv.Atoi(string(buf[1:]))
		if e != nil {
			panic(newSystemErrorWithCause("readMultiBulkData - Atoi parse error", e))
		}
		data[i] = readBulkData(conn, size)
	}
	return data
}
