#ifndef THREAD_H
#define THREAD_H

#include <boost/function.hpp>
#include <boost/noncopyable.hpp>
#include <boost/shared_ptr.hpp>
#include <pthread.h>

#include "Atomic.h"
#include "Timestamp.h"

#include <string.h>

using namespace std;
class Thread: boost::noncopyable
{
public:
	typedef boost::function<void ()> ThreadFunc;

	explicit Thread(const ThreadFunc&,const string& name=std::string());
	~Thread();

	void start();
	int join();

	bool started() const {return started_;}

private:
	bool started_;
	bool joined_;
	pthread_t pthreadId_;
	boost::shared_ptr<pid_t> tid_;
	ThreadFunc func_;
	std::string	name_;

	static AtomicInt32 numCreated_;
};

#endif