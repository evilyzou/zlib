package zredis

import (
	"bufio"
	"fmt"
	"net"
	"time"
)

type ConnInfo struct {
	host     string        // redis connection host
	port     int           // redis connection port
	password string        // redis connection password
	db       int           // Redis connection db #
	rBufSize int           // tcp read buffer size
	wBufSize int           // tcp write buffer size
	rTimeout time.Duration // tcp read timeout
	wTimeout time.Duration // tcp write timeout
}

type RedisConn struct {
	connInfo *ConnInfo
	conn     net.Conn
	reader   *bufio.Reader
}

func DefaultConnInfo() *ConnInfo {
	return &ConnInfo{
		"127.0.0.1",
		5379,
		"",
		1,
		4096,
		4096,
		10 * time.Second,
		10 * time.Second
	}
}

func NewConn(connInfo *ConnInfo) (redisConn *RedisConn) {
	funcInfo := "newConn"

	redisConn = new(RedisConn)
	if redisConn == nil {
		panic(fmt.Errorf("%s: failed to allocate connHdl", funcInfo))
	}

	var mode, addr string
	if connInfo.port == 0 {
		mode = "unix"
		addr = connInfo.host
	} else {
		mode = "tcp"
		addr = fmt.Sprintf("%s:%d", connInfo.host, connInfo.port)
		_, err := net.ResolveTCPAddr("tcp", addr)
		if err != nil {
			panic(fmt.Errorf("%s(): failed to resolve remote address %s", funcInfo, addr))
		}
	}

	conn, e := net.Dial(mode, addr)
	switch {
	case e != nil:
		panic(fmt.Errorf("%s(): could not open connection", funcInfo))
	case conn == nil:
		panic(fmt.Errorf("%s(): net.Dial returned nil, nil (?)", funcInfo))
	default:
		if tcp, ok := conn.(*net.TCPConn); ok {
			tcp.SetReadBuffer(connInfo.rBufSize)
			tcp.SetWriteBuffer(connInfo.wBufSize)
		}
		redisConn.conn = conn
		redisConn.connInfo = connInfo
		bufsize := 4096
		redisConn.reader = bufio.NewReaderSize(conn, bufsize)
	}
	return
}

func (c *RedisConn) ProcessRequest(cmd *Command, key string) (resp Response, err Error) {

	args := [][]byte{[]byte(key)}
	buff := CreateRequestBytes(cmd, args)
	sendRequest(c.conn, buff)

	resp, err = GetResponse(c.reader, cmd)

	return
}
