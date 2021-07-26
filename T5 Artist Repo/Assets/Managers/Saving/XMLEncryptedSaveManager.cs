using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

public class XMLEncryptedSaveManager
{

	// XML SAVING
	public static void Save<T>(T obj, string path)
	{
		string sKey = "Testing1";

		var serializer = new XmlSerializer(obj.GetType());
		var stream = new FileStream(path, FileMode.Create);

		//Create Encryption stuff
		DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
		DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
		DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

		ICryptoTransform desencrypt = DES.CreateEncryptor();

		using (CryptoStream cStream = new CryptoStream(stream, desencrypt, CryptoStreamMode.Write))
		{
			serializer.Serialize(cStream, obj);
		}

		stream.Close();

	}
	public static void Load<T>(ref T obj, string path)
	{
		string sKey = "Testing1";

		var serializer = new XmlSerializer(obj.GetType());
		var stream = new FileStream(path, FileMode.Open);

		//Create Encryption stuff
		DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
		DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
		DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

		ICryptoTransform desdecrypt = DES.CreateDecryptor();

		using (CryptoStream cStream = new CryptoStream(stream, desdecrypt, CryptoStreamMode.Read))
		{
			obj = (T)serializer.Deserialize(cStream);
		}

		stream.Close();
	}
}
