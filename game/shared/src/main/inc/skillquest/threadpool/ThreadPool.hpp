/**
 * @author omnisudo
 * @date 2023.10.11
 */

#pragma once

#include <condition_variable>
#include <mutex>
#include <vector>
#include <queue>
#include <functional>
#include <unordered_map>
#include <thread>

namespace skillquest::threadpool {
    class ThreadPool {
    public:
		~ThreadPool();
		
        void start( uint32_t count = 1 );
        void enqueue(const std::function<void()>& job, std::string name = "" );
        void stop();
        bool busy();
		void wait();
	    
	    bool active();
    
    private:
        void loop();

        bool should_terminate = false;           // Tells threads to stop looking for jobs
        std::mutex queue_mutex;                  // Prevents data races to the job queue
        std::condition_variable mutex_condition; // Allows threads to wait on new jobs or termination
		std::condition_variable wait_condition;
        std::unordered_map<std::thread::id, std::thread*> threads;
        std::queue<std::pair< std::string, std::function<void()>>> jobs;
    };
}
