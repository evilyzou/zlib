#include <iostream>
#include <sys/socket.h>
#include <sys/epoll.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <fcntl.h>
#include <unistd.h>
#include <stdio.h>
#include <pthread.h>
#include <errno.h>
#include <stdlib.h>
#include <string.h>

#define MAXLINE 100
#define OPEN_MAX 100
#define LISTENQ 20
#define SERV_PORT 5555
#define INFTIM 1000

#define BUFFER_SIZE 4096
struct task
{
	int fd;
	struct task *next;
};

struct user_data
{
	int fd;
	unsigned int n_size;
	char buffer[BUFFER_SIZE];
};

void *readtask(void *args);
void *writetask(void *args);

struct epoll_event ev,events[20];

int epfd;
pthread_mutex_t mutex;
pthread_cond_t cond1;


struct task *readhead = NULL,*readtail = NULL, *writehead = NULL;

void setnonblocking(int sock)
{
	int opts ;
	opts = fcntl(sock,F_GETFL);
	if (opts < 0)
	{
		perror("fcntl(sock,GETFL)");
		exit(1);
	}
	opts = opts | O_NONBLOCK;
	if (fcntl(sock,F_SETFL,opts) < 0)
	{
		perror("fcntl(sock,SETFL,opts)");
		exit(1);
	}
}

int main()
{
	int i,maxi,listenfd,connfd,sockfd,nfds;
	pthread_t tid1,tid2;

	struct task *new_task = NULL;
	struct user_data *rdata = NULL;
	socklen_t client;

	pthread_mutex_init(&mutex,NULL);
	pthread_cond_init(&cond1,NULL);

	pthread_create(&tid1,NULL,readtask,NULL);
	//pthread_create(&tid2,NULL,readtask,NULL);

	epfd = epoll_create(256);

	struct sockaddr_in clientaddr;
	struct sockaddr_in serveraddr;

	listenfd = socket(AF_INET,SOCK_STREAM,0);
	setnonblocking(listenfd);

	ev.data.fd = listenfd;
	ev.events = EPOLLIN | EPOLLET;

	epoll_ctl(epfd,EPOLL_CTL_ADD,listenfd,&ev);
	bzero(&serveraddr,sizeof(serveraddr));
	serveraddr.sin_family =AF_INET;
	char *local_addr ="127.0.0.1";
	inet_aton(local_addr,&(serveraddr.sin_addr));
	serveraddr.sin_port = htons(SERV_PORT);
	bind(listenfd,(sockaddr *)&serveraddr,sizeof(serveraddr));
	listen(listenfd,LISTENQ);
	maxi = 0;

	for(;;)
	{
		//wait epoll event happen
		nfds = epoll_wait(epfd,events,20,500);
		printf("count:%d\n",nfds);
		for ( i = 0;i<nfds;++i)
		{
			if (events[i].data.fd == listenfd)
			{
				connfd = accept(listenfd,(sockaddr *)&clientaddr,&client);
				if (connfd < 0)
				{
					perror("connfd <0");
					exit(1);
				}
				setnonblocking(connfd);
				char *str = inet_ntoa(clientaddr.sin_addr);
				std::cout << "connec_ from >>" <<str << std::endl;

				//设置读
				ev.data.fd = connfd;
				ev.events = EPOLLIN | EPOLLET;
				epoll_ctl(epfd,EPOLL_CTL_ADD,connfd,&ev); 
			} else if(events[i].events & EPOLLIN)
			{
				if ((sockfd = events[i].data.fd) < 0)
					continue;

				printf("read data from fd:%d\n",sockfd);
				new_task = new task();
				new_task->fd = sockfd;
				new_task->next = NULL;

				pthread_mutex_lock(&mutex);
				if (readhead == NULL)
				{
					readhead = new_task;
					readtail = new_task;
				} else 
				{
					readtail->next = new_task;
					readtail = new_task;
				}

				pthread_cond_broadcast(&cond1);
				pthread_mutex_unlock(&mutex);
				
			} else if(events[i].events & EPOLLOUT)
			{
				rdata = (struct user_data*)events[i].data.ptr;
				sockfd = rdata->fd;
				//printf("write :%s\n",rdata->buffer);
				//write(sockfd,rdata->line,rdata->n_size);
				write(sockfd,rdata->buffer,BUFFER_SIZE);
				//printf("write pong\n");
				delete rdata;
				ev.data.fd = sockfd;
				ev.events = EPOLLIN | EPOLLET;
				epoll_ctl(epfd,EPOLL_CTL_MOD,sockfd,&ev);
			} else {
				printf("error \n");
			}
		}
	}
}

void *readtask(void *args)
{
	int fd = -1;
	unsigned int n;
	struct user_data *data = NULL;

	while(1)
	{
		pthread_mutex_lock(&mutex);
		while(readhead == NULL)
			pthread_cond_wait(&cond1,&mutex);

		fd = readhead->fd;
		struct task *tmp = readhead;
		readhead = readhead->next;
		delete tmp;
		pthread_mutex_unlock(&mutex);
		data = new user_data();
		data->fd = fd;
		if ((n = read(fd,data->buffer,BUFFER_SIZE)) < 0)
		{
			if (errno == ECONNRESET)
			{
				close(fd);
			} else
			{
				std::cout << "readline error" << std::endl;
				if (data != NULL) delete data;
			} 
		}else
		{
			if (n == 0)
			{
				close(fd);
				printf("Client close connect!\n");
				if (data != NULL) delete data;
			}else 
			{
				printf("n=%d\n",n);
				data->n_size = n;
				ev.data.ptr = data;
				//printf("read :%s\n",data->buffer);
				ev.events = EPOLLOUT| EPOLLET;
				epoll_ctl(epfd,EPOLL_CTL_MOD,fd,&ev);
			}
			
		}
	}
}