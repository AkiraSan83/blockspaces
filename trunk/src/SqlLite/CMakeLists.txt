cmake_minimum_required (VERSION 2.6)
project (sqlite3)
add_library(sqlite3 SHARED sqlite3.c)
add_executable(sqlite3shell shell.c sqlite3.c)