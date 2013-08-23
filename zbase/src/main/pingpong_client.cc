#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <time.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

int main(int argc, char** argv)
{
	int sockfd,portno,n;
	struct sockaddr_in server_addr;
	struct hostent *server;
	char buff[1];
	time_t start,end;
	int i;

	if (argc != 3)
	{
		printf("Usage:\n");
		printf("%s <server_ip> <port>\n",argv[0]);
		return 1;
	}

	char* ipAddress = argv[1];
	char* port = argv[2];

	printf("IpAddress:%s\n",ipAddress);
	printf("Port :%s\n",port);

	portno = atoi(port);
	sockfd = socket(AF_INET,SOCK_STREAM,0);
	if (sockfd < 0)
		printf("error -socket open\n");

	memset(&server_addr,0,sizeof(server_addr));
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.s_addr = inet_addr(ipAddress);
	server_addr.sin_port = htons(portno);
	if (0 > connect(sockfd,(struct sockaddr*)&server_addr,sizeof(server_addr)))
		printf("error -connect\n");

	start = time(0);
	for(i =0;i<200000;i++)
	{
		write(sockfd,buff,1);
		read(sockfd,buff,1);
	}
	end = time(0);
	printf("finished in %d sec\n",end-start);

	return 0;
}