#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <stdio.h>
#include <errno.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

#define BUFFER_SIZE 4096

int main()
{
	int connect_fd;
	int ret; 
	int port = 5555;
	struct sockaddr_in srv_addr;
	char buffer[BUFFER_SIZE] = {"this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
								this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
								this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
								this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
								this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
								this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!\
								this is a test data!this is a test data!this is a test data!this is a test data!this is a test data!"};

	time_t start,end;

	connect_fd = socket(AF_INET,SOCK_STREAM,0);

	if (connect_fd < 0)
	{
		perror("cannot create communication socket");
		return 1;
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
		return 1;
	}
	sleep(5);
	start = time(0);
	for(int i =0;i <2;i++) 
	{
		write(connect_fd,buffer,BUFFER_SIZE);
		read(connect_fd,buffer,BUFFER_SIZE);
	}
	end = time(0);
	printf("finished in %d secs\n", end - start);
	sleep(10);
	return 0;
} 