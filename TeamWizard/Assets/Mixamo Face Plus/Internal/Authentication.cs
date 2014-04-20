using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.IO;
using System.Net;
using System.Text;
using System;

namespace Mixamo {

	enum LoginCode {
		InternalError = -1,
		Success = 0,
		Forbidden = 401,
		ServerError = 500
	}

	public class Authentication {

		public class UserRecord {
			public string Email;
		}

		public static UserRecord User;

		public static bool IsAuthenticated {
			get {
				return (User != null);
			}
		}

		public static bool CanUseFacePlus {
			get {
				return IsAuthenticated;
			}
		}

		public static bool IsLoggingIn = false;

		public static void Logout() {
			User = null;
		}

		public static void Login(string user, string password, Action<string> onSuccess, Action<string> onFailure) {

			if(IsLoggingIn) return;

			IsLoggingIn = true;

			Thread loginThread = new Thread(() =>{
				int result;

				result = FacePlus.Login (user, password);

				if(!Enum.IsDefined(typeof(LoginCode), result)) {
					onFailure("Login Failed (Server Code: "+result+").");
				} else {
					switch((LoginCode)result) {
						case LoginCode.Success:
							User = new UserRecord() {
								Email = user
							};
							onSuccess("");
							break;

						case LoginCode.Forbidden:
							onFailure("Incorrect username or password.");
							break;

						case LoginCode.InternalError:
							onFailure("Login Failed, Internal Error (" + result +").");
							break;

						case LoginCode.ServerError:
							onFailure("Server Error (500). Try again later.");
							break;

						default:
							onFailure("Login Failed (Server Code: "+result+").");
							break;
					}
				}

				IsLoggingIn = false;
			});
			loginThread.Start ();
			while(!loginThread.IsAlive) {}
		}
	}
}
