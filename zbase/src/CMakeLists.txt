set(CMAKE_CXX_COMPILER g++)

include_directories(${HELLO_SOURCE_DIR}/include)

add_subdirectory(zbase)
add_subdirectory(main)

