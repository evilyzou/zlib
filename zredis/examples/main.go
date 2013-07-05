package main

import (
	"fmt"
	"zlib/zredis"
)

func main() {
	connInfo := &zredis.ConnInfo{"127.0.0.1", 5379, "", 0, 4096, 4096, nil, nil}
	t := zredis.NewConn(connInfo).ProcessRequest(GET, "123")
	fmt.Println("test", t)
}
