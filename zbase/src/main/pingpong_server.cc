#include <stdio.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <time.h>
#include <unistd.h>
#include <stdlib.h>
#include <string.h>

int main(int argc,char ** argv)
{
	int sockfd,newsockfd,portno;
	socklen_t client;
	char buffer[1];
	struct sockaddr_in serv_addr,cli_addr;
	int n;

	if (argc < 2)
	{
		printf("Usage:\n");
		printf("%s <port>\n",argv[0]);
		return 1;
	}

	portno = atoi(argv[1]);
	sockfd = socket(AF_INET,SOCK_STREAM,0);
	if (sockfd < 0)
		printf("error - socket open\n");

	memset(&serv_addr,0,sizeof(serv_addr));
	serv_addr.sin_family = AF_INET;
	serv_addr.sin_addr.s_addr = INADDR_ANY;
	serv_addr.sin_port = htons(portno);

	if (0 >bind(sockfd,(struct sockaddr *)&serv_addr,sizeof(serv_addr)))
		printf("error -bind\n");

	listen(sockfd,5);
	printf("waiting for connection...\n");
	newsockfd = accept(sockfd,(struct sockaddr *)&cli_addr,&client);
	if (newsockfd < 0)
		printf("error -accept\n");

	printf("connected\n");

	while(1)
	{
		read(newsockfd,buffer,1);
		write(newsockfd,buffer,1);
	}
	return 0;
}