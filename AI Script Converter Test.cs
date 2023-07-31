using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Linq;
using System.IO;
using System;

namespace Ayehu.Sdk.ActivityCreation
{
	public class CustomActivity : IActivity
	{
		public string inputCode;
		public string source;
		public string target;
		public string output;

		public ICustomActivityResult Execute()
		{
			string apiKey = "--------------------------";
			string apiUrl = "https://api.openai.com/v1/chat/completions";
			string jsonBody = @"
			{
				""model"": ""gpt-3.5-turbo"",
				""messages"": [
					{
						""role"": ""system"",
						""content"": ""You are a genius level coder, with knowledge of all programming and scripting languages and their associated syntax.""
					},
					{
						""role"": ""system"",
						""content"": ""Translate the code from [" + source + @"] to [" + target + @"]""
					},
					{
						""role"": ""user"",
						""content"": """ + HttpUtility.JavaScriptStringEncode(inputCode) + @"""
					}
					
				],
				""temperature"": 1.0,
				""max_tokens"": 150
			}";

			try
			{
				System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

				var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Headers.Add("Authorization", "Bearer " + apiKey);
				httpWebRequest.Method = "POST";

				using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
				{
					streamWriter.Write(jsonBody);
					streamWriter.Flush();
					streamWriter.Close();

					var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

					using(var streamReader = new StreamReader(httpResponse.GetResponseStream()))
					{
						var response = streamReader.ReadToEnd();

						if(output == "Converted Code")
						{
							JObject jsonResults = JObject.Parse(response);
							
							return this.GenerateActivityResult(jsonResults["choices"][0]["message"]["content"].ToString());
						}
						else
						{
							return this.GenerateActivityResult(response.ToString());
						}
					}
				}
			}
			catch(WebException e)
			{
				throw new Exception(e.Message);
			}
		}
	}
}
