using System;
using System.Threading.Tasks;

namespace Sandbox.SQ;

//TODO: Send which function should be called safely

public static class Networking {
	public static class Async {
		public static Task Host ( params object[] args ) {
			var guid  = Guid.NewGuid();
			var tcs = _noreturn [ guid ] = new TaskCompletionSource();

			__process_sv( guid, args );

			return tcs.Task;

		}
	}

	public static class Async < TRet > {
		public static Task< TRet > Host ( params object[] args  ) {
			var guid = Guid.NewGuid();
			
			var tcs = _withreturn [ guid ] = new TaskCompletionSource<object>();
			
			__process_sv( guid, args );
			
			return Task.Run< TRet >( async ( ) => ( TRet ) await tcs.Task );
		}
	}

	[ Rpc.Host ]
	private static void __process_sv ( Guid guid, object[] args ) {
		
	}

	[ Rpc.Broadcast ]
	private static void __execute_cl ( Guid guid, object ret ) {
		
	}

	[ Rpc.Broadcast ]
	private static void __process_cl ( Guid guid, object[] args ) {
		
	}

	[ Rpc.Host ]
	private static void __execute_sv ( Guid guid, object ret ) {
		
	}

	private static Dictionary< Guid, TaskCompletionSource< object > > _withreturn = new();
	private static Dictionary< Guid, TaskCompletionSource > _noreturn = new();
}
