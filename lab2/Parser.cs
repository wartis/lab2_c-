using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace lab2
{
    class Parser
    {
        public List<Threat> ParseExcelToThreatsList(string pathToFile)
        {
            var list = new List<Threat>();
            FileStream stream = null;
            IExcelDataReader excelReader = null;
            try
            {
                stream = File.Open(pathToFile, FileMode.Open, FileAccess.Read);
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                excelReader.Read(); //название таблицы
                excelReader.Read(); //заголовки 
                while (excelReader.Read())
                {
                    var obj = new Threat()
                    {
                        Id = (int)excelReader.GetDouble(0),
                        Name = excelReader.GetString(1),
                        Description = excelReader.GetString(2),
                        Source = excelReader.GetString(3),
                        ImpactObject = excelReader.GetString(4),
                        isBrokenConfidentiality = excelReader.GetDouble(5) == 1 ? true : false,
                        isBrokenIntegrity = excelReader.GetDouble(6)       == 1 ? true : false,
                        isBrokenAvailobility = excelReader.GetDouble(7)    == 1 ? true : false
                    };

                    list.Add(obj);
                }
                excelReader.Close();
                return list;
            }
            catch (Exception ex)
            {
                excelReader.Close(); 
                throw new Exception("Не удалось пропарсить эксель файл!");
            }
            

            
        }



    }
}
