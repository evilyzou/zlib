package zredis

type RequestType int

const (
	_ RequestType = iota
	NO_ARG
	KEY
	KEY_KEY
	KEY_NUM
	KEY_SPEC
	KEY_NUM_NUM
	KEY_VALUE
	KEY_IDX_VALUE
	KEY_KEY_VALUE
	KEY_CNT_VALUE
	MULTI_KEY
)

type ResponseType int

const (
	VIRTUAL ResponseType = iota
	BOOLEAN
	NUMBER
	STRING
	STATUS
	BULK
	MULTI_BULK
)

type Command struct {
	Code     string
	ReqType  RequestType
	RespType ResponseType
}

var (
	AUTH Command = Command{"AUTH", KEY, STATUS}
	PING Command = Command{"PING", NO_ARG, STATUS}
	QUIT Command = Command{"QUIT", NO_ARG, VIRTUAL}
	SET  Command = Command{"SET", KEY_VALUE, STATUS}
	GET  Command = Command{"GET", KEY, BULK}
)
