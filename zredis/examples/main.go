package main

import (
	"fmt"
	"time"
	"zlib/zredis"
)

func main() {
	connInfo := &zredis.ConnInfo{"127.0.0.1", 5379, "", 0, 4096, 4096, 8 * time.Second, 8 * time.Second}
	t := zredis.NewConn(connInfo).ProcessRequest(GET, "123")
	fmt.Println("test", t)
}
