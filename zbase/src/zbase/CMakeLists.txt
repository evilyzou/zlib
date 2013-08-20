set(SRC_LIST 
	Thread.cc
	Timestamp.cc)

add_library(zbase STATIC ${SRC_LIST})
target_link_libraries(zbase pthread)

#install(TARGETS zbase DESTINATION lib)