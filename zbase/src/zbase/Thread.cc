#include <boost/static_assert.hpp>
#include <boost/type_traits/is_same.hpp>
#include <boost/weak_ptr.hpp>


#include <errno.h>
#include <stdio.h>
#include <unistd.h>
#include <sys/syscall.h>
#include <sys/types.h>
#include <linux/unistd.h>

#include "Thread.h"
#include "CurrentThread.h"

//using namespace CurrentThread;

namespace detail
{
	pid_t gettid()
	{
		return static_cast<pid_t>(::syscall(SYS_gettid));
	}

	void afterFork()
	{
		CurrentThread::t_cachedTid = 0;
		CurrentThread::t_threadName = "main";
		CurrentThread::tid();
	}

	class ThreadNameInitializer
	{
	public:
		ThreadNameInitializer()
		{
			CurrentThread::t_threadName = "main";
			CurrentThread::tid();
			pthread_atfork(NULL,NULL,&afterFork);
		}
	};
	ThreadNameInitializer init;

	struct ThreadData
	{
		typedef Thread::ThreadFunc ThreadFunc;
		ThreadFunc func_;
		std::string name_;
		boost::weak_ptr<pid_t> wkTid_;

		ThreadData(const ThreadFunc& func,
				   const std::string& name,
				   const boost::shared_ptr<pid_t>& tid)
		:func_(func),
		name_(name),
		wkTid_(tid)
		{}

		void runInThread()
		{
			pid_t tid  = CurrentThread::tid();

			boost::shared_ptr<pid_t> ptid = wkTid_.lock();
			if (ptid)
			{
				*ptid = tid;
				ptid.reset();
			}

			CurrentThread::t_threadName = name_.c_str();
			try 
			{
				func_();
				CurrentThread::t_threadName = "finished";
			}
			/*
			catch(const Exception& ex)
			{
				CurrentThread::t_threadName ="crashed";
				fprintf(stderr,"exception caught in Thread %s\n",name_.c_str());
				fprintf(stderr, "reason: %s\n", ex.what());
      			fprintf(stderr, "stack trace: %s\n", ex.stackTrace());
      			abort();
			}
			*/
			catch (...)
		    {
		      CurrentThread::t_threadName = "crashed";
		      fprintf(stderr, "unknown exception caught in Thread %s\n", name_.c_str());
		      throw; // rethrow
		    }
		}
	};

	void* startThread(void * obj)
	{	
		ThreadData* data  = static_cast<ThreadData*>(obj);
		data->runInThread();
		delete data;
		return NULL;
	}
};



namespace CurrentThread
{
	__thread int t_cachedTid;
	__thread char t_tidString[32];
	__thread const char* t_threadName = "unknown";
	const bool sameType = boost::is_same<int,pid_t>::value;
	BOOST_STATIC_ASSERT(sameType);
}

void CurrentThread::cacheTid()
{
	if (t_cachedTid == 0)
	{
		t_cachedTid = detail::gettid();
		int n = snprintf(t_tidString,sizeof (t_tidString),"%5d",t_cachedTid);
		//printf("t_cachedTid=%d\n",n);
		assert(n == 5); (void)n;
	}
}

bool CurrentThread::isMainThread()
{
	return tid() == ::getpid();
}

void CurrentThread::sleepUsec(int64_t usec)
{
	struct timespec ts = {0,0};
	ts.tv_sec = static_cast<time_t>(usec / Timestamp::kMicroSecondsPerSecond);
  	ts.tv_nsec = static_cast<long>(usec % Timestamp::kMicroSecondsPerSecond * 1000);
  	::nanosleep(&ts, NULL);
}

AtomicInt32 Thread::numCreated_;
	
Thread::Thread(const ThreadFunc& func,const string& n)
	:started_(false),
	joined_(false),
	pthreadId_(0),
	tid_(new pid_t(0)),
	func_(func),
	name_(n)
{
	numCreated_.increment();
}

Thread::~Thread()
{
	if (started_ && !joined_)
	{
		pthread_detach(pthreadId_);
	}
}

void Thread::start()
{
	assert(!started_);
	started_ = true;
	detail::ThreadData* data = new detail::ThreadData(func_,name_,tid_);
	if (pthread_create(&pthreadId_,NULL,&detail::startThread,data))
	{
		started_ = false;
		delete data;
		printf("Failed in pthread_create\n");
	}
}

int Thread::join()
{
	assert(started_);
	assert(!joined_);
	joined_ = true;
	return pthread_join(pthreadId_,NULL);
}