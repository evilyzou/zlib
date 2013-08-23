#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <stdio.h>
#include <errno.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <pthread.h>

#define BUFFER_SIZE 4096
#define THREAD_COUNT 2

#define BUFFER_CONTENT "this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
						this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
						this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
						this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
						this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
						this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
						this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!"
/*
* bench 测试,起N个线程测试
*/

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
pthread_cond_t cond = PTHREAD_COND_INITIALIZER;
static int count = THREAD_COUNT;

void* threadFunc(void* data)
{
	int connect_fd;
	int ret; 
	int port = 5555;
	pthread_t tid = pthread_self();
	struct sockaddr_in srv_addr;
	char buffer[BUFFER_SIZE] = BUFFER_CONTENT;

	time_t start,end;

	connect_fd = socket(AF_INET,SOCK_STREAM,0);

	if (connect_fd < 0)
	{
		perror("cannot create communication socket");
		exit(1);
	} 
	memset(&srv_addr,0,sizeof(srv_addr));
	srv_addr.sin_family = AF_INET;
	srv_addr.sin_addr.s_addr = inet_addr("127.0.0.1");
	srv_addr.sin_port = htons(port);
	ret = connect(connect_fd,(struct sockaddr*)&srv_addr,sizeof(srv_addr));

	if ( -1 == ret)
	{
		perror("cannot connect to the server");
		printf("[CONNECT_ERROR]%s\n",strerror(errno));
		exit(1);
	}

	/*
	pthread_mutex_lock(&mutex);
	count --;
	if (count == 0)
		pthread_cond_broadcast(&cond);

	while(count >0 ){
		pthread_cond_wait(&cond,&mutex);	
	}
	
	pthread_mutex_unlock(&mutex);
	*/
	printf("start thread id:%u\n",tid);
	start = time(0);
	for(int i =0;i <2;i++) 
	{
		int n = write(connect_fd,buffer,BUFFER_SIZE);
		printf("write length:%d\n",n);
		printf("thread id:%u write end\n",(unsigned int)tid);
		read(connect_fd,buffer,BUFFER_SIZE);
		printf("thread id:%u read end\n",(unsigned int)tid);
	}
	end = time(0);
	printf("thread id:%u finished in %d secs\n",(unsigned int)tid, end - start);
}

int main()
{
	int i,err;
	pthread_t t[THREAD_COUNT];
	for(i=0;i<THREAD_COUNT;i++)
	{
		err = pthread_create(&t[i],NULL,&threadFunc,NULL);
		if (err != 0) 
		{
			perror("pthread_create failed!\n");
			return 1;
		}
	}

	for(i=0;i<THREAD_COUNT;i++)
	{
		pthread_join(t[i],NULL);
	}


	return 0;
}

