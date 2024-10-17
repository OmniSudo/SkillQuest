/**
 * @author omnisudo
 * @date 2023.10.11
 */

#include "skillquest/threadpool/ThreadPool.hpp"
#include <future>

namespace skillquest::threadpool {
	ThreadPool::~ThreadPool() {
		should_terminate = true;
		mutex_condition.notify_all();
	}
	
	
	void ThreadPool::start( uint32_t count ) {
		const uint32_t num_threads = count; // Max # of threads the system supports
		for (uint32_t ii = 0; ii < num_threads; ++ii) {
			std::promise< std::thread::id > promise;
			auto future = promise.get_future();
			auto thread = new std::thread(
					[ this, &promise ]() {
						promise.set_value( std::this_thread::get_id());
						this->loop();
					}
			);
			future.wait();
			threads[future.get()] = thread;
			thread->detach();
		}
	}
	
	void ThreadPool::loop() {
		while (true) {
			std::function < void() > job;
			
			std::unique_lock< std::mutex > lock( queue_mutex );
			mutex_condition.wait( lock, [ this ] {
				return !jobs.empty() || should_terminate;
			} );
			if (should_terminate) {
				return;
			}
			job = jobs.front().second;
			
			job();
			jobs.pop();
			wait_condition.notify_all();
		}
	}
	
	// TODO: Use <future> to wait() until a job is completed
	void ThreadPool::enqueue( const std::function< void() > &job, std::string name ) {
		{
			jobs.push( std::make_pair( name, job ));
		}
		mutex_condition.notify_one();
	}
	
	bool ThreadPool::busy() {
		bool poolbusy;
		{
			std::unique_lock< std::mutex > lock( queue_mutex );
			poolbusy = !jobs.empty();
		}
		return poolbusy;
	}
	
	void ThreadPool::stop() {
		{
			std::unique_lock< std::mutex > lock( queue_mutex );
			should_terminate = true;
		}
		mutex_condition.notify_all();
		wait_condition.notify_all();
		for (auto &active_thread: threads) {
			delete active_thread.second;
		}
		threads.clear();
	}
	
	void ThreadPool::wait() {
		for (auto &thread: this->threads) {
			if (active()) {
				throw std::runtime_error{"Tried to wait a thread within itself"};
			}
		}
		std::unique_lock< std::mutex > lock( queue_mutex );
		wait_condition.wait( lock, [ this ] {
			return jobs.empty() || should_terminate;
		} );
	}
	
	auto ThreadPool::active() -> bool {
		for (auto &thread: threads) {
			if (thread.first == std::this_thread::get_id()) {
				return true;
			}
		}
		return false;
	}
}