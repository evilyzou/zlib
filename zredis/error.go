package zredis

import (
	"fmt"
)

// supports SystemError interface
type systemError struct {
	msg   string
	cause error
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
