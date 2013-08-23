add_executable(main main.cc)
target_link_libraries(main zbase)

add_executable(Mutex_test Mutex_test.cc)
target_link_libraries(Mutex_test zbase)

add_executable(epoll_test_server epoll_test_server.cc)
target_link_libraries(epoll_test_server pthread)

add_executable(epoll_test_client epoll_test_client.cc)

add_executable(pingpong_server pingpong_server.cc)
add_executable(pingpong_client pingpong_client.cc)
add_executable(epoll_benchmark epoll_benchmark.cc)
target_link_libraries(epoll_benchmark pthread)