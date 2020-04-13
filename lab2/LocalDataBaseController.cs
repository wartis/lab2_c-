using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace lab2
{
    class LocalDataBaseController
    {

        public List<string> ChangesList { get; private set; } = new List<string>() { "Локальная база данных не обновлялась." };
        public int PageNumber { get; private set; }
        public int AmountOfPages { get; private set; }

        private int recordsInOnePage = 20;
        private string fileName = @"\dontChangeItPlease.xlsx"; 
        private string localDbPathDirectory    = @"C:\laba"; 
        private string uriOfExcelFile = "https://bdu.fstec.ru/files/documents/thrlist.xlsx";
        
        private List<Threat> localDb;

        /// <summary>
        /// Method returns the first page of the local threats data base. If it hasn't been created yet, creat it firstly. 
        /// </summary>
        /// <returns>ObservableCollection</returns>
        public List<Threat> GetLocalDB()
        {

            if (File.Exists(localDbPathDirectory + fileName)) localDb = LoadLocalDB();
            else 
            {
                if (Directory.Exists(localDbPathDirectory)) localDb = CreateLocalDB();
                else
                {
                    Directory.CreateDirectory(localDbPathDirectory);
                    localDb = CreateLocalDB(); 
                }
            }  
                

            if (localDb != null)
            {
                AmountOfPages = localDb.Count % recordsInOnePage == 0 ? localDb.Count / 20 : (localDb.Count / 20) + 1;
                PageNumber = 1;  
            }

            return GetPage(1);
        }

        public List<Threat> NextPage() 
        {
            PageNumber++; 
            return GetPage(PageNumber); 
        }

        public List<Threat> PreviousPage()
        {
            PageNumber--;
            return GetPage(PageNumber);
        }

        public List<Threat> GetPage(int pageNum) 
        {
            List<Threat> page = new List<Threat>();
            PageNumber = pageNum; 

            if (pageNum != AmountOfPages)
            {
                for (int i = 0; i < recordsInOnePage; i++)
                {
                    Threat record = localDb[(pageNum-1) * recordsInOnePage + i];
                    page.Add(record);
                }
            }
            else 
            {
                int recordsInLastPage = localDb.Count - recordsInOnePage * (pageNum-1);
                for (int i = 0; i < recordsInLastPage; i++)
                {
                    Threat record = localDb[localDb.Count - recordsInLastPage + i];
                    page.Add(record);
                }
            }

            return page;
        }

        public bool CopyLocalDbToFile(string wherePath)
        {
            try
            {
                string destFile = Path.Combine(@"", wherePath + fileName);
                File.Copy(localDbPathDirectory + fileName, destFile, true);
                return true; 
            }
            catch (Exception ex)
            {
                return false; 
            }
        }

        public void UpdateLocalDb() 
        {
            List<Threat> newLocalDb = CreateLocalDB();

            ChangesList = FindChanges(newLocalDb);

            localDb = newLocalDb; 
        }

       
        public void AddHiddenThreat() 
        {
            if (localDb == null) return;
            Threat hiddenThreat = new Threat();
            hiddenThreat.Name = "Скрытая угроза!";
            hiddenThreat.Id = 0;
            hiddenThreat.Description = "Неспокойные времена настали для Галактической Республики. Налогообложение торговых путей к отдаленным солнечным системам стало причиной раздоров." +
                                        "В стремлении добиться своего обуянная алчностью Торговая Федерация с помощью мощных боевых кораблей взяла в кольцо блокады маленькую планету Набу, лишив её всех поставок." +
                                        "В то время как члены Конгресса Республики ведут напряженные дебаты в связи с тревожными событиями, Верховный канцлер втайне от всех поручил двум рыцарям-джедаям — хранителям мира и справедливости в Галактике — урегулировать конфликт…";
            localDb.Add(hiddenThreat); 
        }

        private List<Threat> CreateLocalDB()
        {
            if (!DownloadExcelFile())
            {
                throw new Exception("Не смогли загрузить файл!");
            }

            List<Threat> list = new Parser().ParseExcelToThreatsList(localDbPathDirectory+fileName);

            return list;
        }

        private bool DownloadExcelFile()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(uriOfExcelFile, localDbPathDirectory + fileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private List<Threat> LoadLocalDB()
        {
            return new Parser().ParseExcelToThreatsList(localDbPathDirectory + fileName);
        }

        private List<string> FindChanges(List<Threat> newLocalDb)
        {
            if (localDb == null) throw new Exception("Сначала загрузите лбд!");
            List<String> list = new List<string>();

            for (int i = 0; i < localDb.Count; i++)
            {
                for (int j = 0; j < newLocalDb.Count; j++)
                {
                    if (localDb[i].Id == newLocalDb[j].Id)
                    {
                        if (localDb[i].Name != newLocalDb[j].Name)
                            list.Add($"УБИ.{localDb[i].Id} (Изменилось имя):\n\t {localDb[i].Name} - {newLocalDb[j].Name}");
                        if (localDb[i].Description != newLocalDb[j].Description)
                            list.Add($"УБИ.{localDb[i].Id} (Изменилось описание):\n\t {localDb[i].Description} - {newLocalDb[j].Description}");
                        if (localDb[i].Source != newLocalDb[j].Source)
                            list.Add($"УБИ.{localDb[i].Id} (Изменился источник угрозы):\n\t {localDb[i].Source} - {newLocalDb[j].Source}");
                        if (localDb[i].ImpactObject != newLocalDb[j].ImpactObject)
                            list.Add($"УБИ.{localDb[i].Id} (Изменился объект воздействия угрозы):\n\t {localDb[i].ImpactObject} - {newLocalDb[j].ImpactObject}");
                        if (localDb[i].isBrokenConfidentiality != newLocalDb[j].isBrokenConfidentiality)
                            list.Add($"УБИ.{localDb[i].Id} (Изменилось нарушение конфиденциальности):\n\t {localDb[i].isBrokenConfidentiality} - {newLocalDb[j].isBrokenConfidentiality}");
                        if (localDb[i].isBrokenIntegrity != newLocalDb[j].isBrokenIntegrity)
                            list.Add($"УБИ.{localDb[i].Id} (Изменилось нарушение целостности):\n\t {localDb[i].isBrokenIntegrity} - {newLocalDb[j].isBrokenIntegrity}");
                        if (localDb[i].isBrokenAvailobility != newLocalDb[j].isBrokenAvailobility)
                            list.Add($"УБИ.{localDb[i].Id} (Изменилось нарушение доступности):\n\t {localDb[i].isBrokenAvailobility} - {newLocalDb[j].isBrokenAvailobility}");
                    }
                }
            }

            if (list.Count == 0) list.Add("Никаких изменений не произошло.");
            else list.Add($"\n\nБыло изменено {list.Count} записей.");

            return list;
        }


    }
}
