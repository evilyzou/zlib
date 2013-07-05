package zredis

import (
	"buffer"
	"bufio"
	"bytes"
	"errors"
	"fmt"
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
