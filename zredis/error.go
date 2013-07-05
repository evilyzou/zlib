package zredis

import (
	"fmt"
	"net"
	"reflect"
)

type Error interface {
	error
	// if true Error is a RedisError
	IsRedisError() bool
}

// supports SystemError interface
type systemError struct {
	msg   string
	cause error
}

type SystemError interface {
	Cause() error
}

type RedisError interface {
	Message() string
}

func newSystemError(msg string) Error {
	return newSystemErrorWithCause(msg, nil)
}
func newSystemErrorf(format string, args ...interface{}) Error {
	return newSystemError(fmt.Sprintf(format, args...))
}

func newSystemErrorWithCause(msg string, cause error) Error {
	e := &systemError{
		msg:   msg,
		cause: cause,
	}
	return e
}

func onRecover(e interface{}, info string) (err Error) {
	if e != nil {
		switch {
		case isSystemError(e):
			return e.(Error)
		case isGenericError(e):
			return newSystemErrorWithCause(info, e.(error))
		default:
			return newSystemErrorf(info+" - recovered: %s", e)
		}
	}
	return nil
}
func isGenericError(e interface{}) bool {
	if e != nil && !(isRedisError(e) || isSystemError(e)) {
		return true
	}
	return false
}
func isSystemError(e interface{}) bool {
	if e != nil && reflect.TypeOf(e).Implements(reflect.TypeOf((*SystemError)(nil)).Elem()) {
		return true
	}

	return false
}

func isRedisError(e interface{}) bool {
	if e != nil && reflect.TypeOf(e).Implements(reflect.TypeOf((*RedisError)(nil)).Elem()) {
		return true
	}
	return false
}
func isNetError(e interface{}) bool {
	if e != nil && reflect.TypeOf(e).Implements(reflect.TypeOf((*net.Error)(nil)).Elem()) {
		return true
	}
	return false
}

func (e *systemError) IsRedisError() bool { return false }
func (e *systemError) Error() string {
	cause := ""
	if e.cause != nil {
		cause = fmt.Sprintf(" [cause: %s]", e.cause.Error())
	}
	return fmt.Sprintf("SYSTEM_ERROR - %s%s", e.msg, cause)
}
