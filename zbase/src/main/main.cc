#include <stdio.h>

#include "Thread.h"
#include "CurrentThread.h"

#include <string>
#include <boost/bind.hpp>
#include <stdio.h>


void threadFunc()
{
	printf("tid:%d\n",CurrentThread::tid());
}

class Foo
{
public:
	explicit Foo(double x)
	:x_(x)
	{

	}

	void memberFunc()
	{
		printf("tid=%d,Foo::x_=%f\n",CurrentThread::tid(),x_);
	}

	void memberFunc1(const std::string text)
	{
		printf("tid=%d,Foo::x_=%f,text=%s\n",CurrentThread::tid(),x_,text.c_str());
	}
private:
		double x_;
};

int main()
{
	printf("pid;%d,tid;%d\n",::getpid(),CurrentThread::tid());

	Thread t1(threadFunc);
	t1.start();
	t1.join();

	Foo foo(12.23);
	Thread t2(boost::bind(&Foo::memberFunc,&foo));
	t2.start();
	t2.join();

	Thread t3(boost::bind(&Foo::memberFunc1,&foo,std::string("test3")));
	t3.start();
	t3.join();


	return 0;
}