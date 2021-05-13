using System;
using System.IO;
using SimpleCalculator.Common.MessageTemplates;

namespace SimpleCalculator.Common.IO
{
	public class FileCommunication: ICommunicationService
	{
		private string _path;

		public string GetInput(string path = null)
		{
			ValidateFileExist(path);
			_path = path;

			return string.Join(Constants.LINE_SEPARATOR, File.ReadAllLines(path));
		}

		public void WriteResponse(string response)
		{
			if (string.IsNullOrEmpty(response))
			{
				throw new ArgumentNullException(nameof(response));
			}
			
			ValidateFileExist(_path);
			File.WriteAllText(_path, response);
		}

		private void ValidateFileExist(string path)
		{
			if (File.Exists(path))
			{
				return;
			}

			throw new FileNotFoundException(Constants.FILE_NOT_FOUND);
		}
	}
}