using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using Data = Google.Apis.Sheets.v4.Data;
using System.Xml;
using System.Device.Location;

namespace PoolScrapper
{
    public class PoolHandler
    {
        public static string spreadSheetId = "1WEfGS-T8jQ31ENi9LQmOyzpp2U7eAX5gGT9-5NMwKzM";
        public static void GetPoolData()
        {
            string response = string.Empty;
            Boolean header = true;
            string sheetName = "Sheet_17_04_2018_12_25";

            var service = CreateGoogleSheets(sheetName);

            List<KeyValuePair<string, string>> competitionList = new List<KeyValuePair<string, string>>();
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://websites.sportstg.com/comp_info.cgi?round=1&a=ROUND&client=0-11971-0-487287-0&pool=1");
                webRequest.Method = "GET";
                webRequest.Host = webRequest.RequestUri.Host;
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                StreamReader stream = new StreamReader(webResponse.GetResponseStream());
                response = stream.ReadToEnd();
                stream.Dispose();

                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(response);

                HtmlNodeCollection competitionNodeList = htmlDoc.DocumentNode.SelectNodes("//select[@id='compselectbox']");

                if (competitionNodeList != null)
                {
                    if (competitionNodeList.Count() > 0)
                    {
                        for (int x = 0; x < competitionNodeList.Count(); x++)
                        {
                            HtmlNodeCollection subCompetitionNodeList = competitionNodeList[x].SelectNodes(".//option");

                            for (int y = 0; y < subCompetitionNodeList.Count(); y++)
                            {
                                string competitionName = subCompetitionNodeList[y].InnerText.Replace("&nbsp;", "").Trim().ToString();
                                string competitionCode = subCompetitionNodeList[y].Attributes["value"].Value.Trim().ToString();
                                if (competitionCode != "")
                                {
                                    string teamUrl = "http://websites.sportstg.com/comp_info.cgi?c=" + competitionCode + "&a=ROUND#";
                                    competitionList.Add(new KeyValuePair<string, string>(competitionName, teamUrl));
                                }
                            }
                        }
                    }
                }

                if (competitionList.Count() > 0)
                {
                    foreach (var item in competitionList)
                    {
                        if (item.Value != "")
                        {
                            List<KeyValuePair<string, string>> poolList = new List<KeyValuePair<string, string>>();
                            List<KeyValuePair<string, string>> roundList = new List<KeyValuePair<string, string>>();
                            string poolResponse = PoolHandler.GetResponseFromUrl(item.Value);

                            HtmlAgilityPack.HtmlDocument htmlnewDoc = new HtmlAgilityPack.HtmlDocument();
                            htmlnewDoc.LoadHtml(poolResponse);

                            HtmlNodeCollection htmlPools = htmlnewDoc.DocumentNode.SelectNodes("//div[@class='fixoptions']//div[@class='nonactpool-wrap']//a");

                            #region Retrive Pools List
                            if (htmlPools != null)
                            {
                                if (htmlPools.Count() > 0)
                                {
                                    for (int j = 0; j < htmlPools.Count(); j++)
                                    {
                                        string poolTitle = htmlPools[j].InnerText.Trim().ToString();
                                        string poolUrl = htmlPools[j].Attributes["href"].Value.ToString();
                                        if (poolTitle != "Final" && poolUrl != "#")
                                        {
                                            poolList.Add(new KeyValuePair<string, string>(poolTitle, "http://websites.sportstg.com/" + poolUrl.Replace("amp;", "").Trim().ToString()));
                                        }
                                    }
                                }
                            }
                            #endregion

                            if (poolList.Count() > 0)
                            {
                                foreach (var poolItem in poolList)
                                {
                                    string pool_Name = poolItem.Key;
                                    if (!string.IsNullOrEmpty(poolItem.Value))
                                    {
                                        string subPoolResponse = PoolHandler.GetResponseFromUrl(poolItem.Value);

                                        HtmlAgilityPack.HtmlDocument htmlnewDocA = new HtmlAgilityPack.HtmlDocument();
                                        htmlnewDocA.LoadHtml(subPoolResponse);

                                        HtmlNodeCollection currentRoundNode = htmlnewDocA.DocumentNode.SelectNodes("//div[@class='roundlist']//span[@data-rd]");
                                        HtmlNodeCollection roundsNodes = htmlnewDocA.DocumentNode.SelectNodes("//div[@class='roundlist']//a[@data-rd]");

                                        if (currentRoundNode != null)
                                        {
                                            if (currentRoundNode.Count() > 0)
                                            {
                                                string roundValue = currentRoundNode[0].Attributes["data-rd"].Value.Trim().ToString();
                                                string roundUrl = poolItem.Value;
                                                if (roundValue != "")
                                                {
                                                    roundList.Add(new KeyValuePair<string, string>("Round " + roundValue, roundUrl.Replace("round=0", "round=" + roundValue).Replace("action=ROUND", "a=ROUND")));
                                                }
                                            }
                                        }

                                        if (roundsNodes != null)
                                        {
                                            if (roundsNodes.Count() > 0)
                                            {
                                                for (int rndCount = 0; rndCount < roundsNodes.Count(); rndCount++)
                                                {
                                                    if (roundsNodes[rndCount].Attributes.Contains("data-rd"))
                                                    {
                                                        string roundValue = roundsNodes[rndCount].Attributes["data-rd"].Value.Trim().ToString();
                                                        string roundUrl = roundsNodes[rndCount].Attributes["href"].Value.Trim().Replace("amp;", "").Trim().ToString();
                                                        if (!string.IsNullOrEmpty(roundValue))
                                                        {
                                                            roundList.Add(new KeyValuePair<string, string>("Round " + roundValue, "http://websites.sportstg.com/" + roundUrl));
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (roundList.Count > 0)
                                        {
                                            foreach (var roundItem in roundList.OrderBy(x => x.Key))
                                            {
                                                string roundResponse = PoolHandler.GetResponseFromUrl(roundItem.Value);

                                                int startIndex = roundResponse.IndexOf("var matches =");
                                                int endIndex = roundResponse.LastIndexOf("];</script>");

                                                if (startIndex > 0 && endIndex > 0)
                                                {
                                                    string jsonString = roundResponse.Substring(startIndex, endIndex - startIndex + 5);
                                                    jsonString = jsonString.Replace("var matches =", "").Replace(";</s", "").Trim().ToString();

                                                    if (!string.IsNullOrEmpty(jsonString))
                                                    {
                                                        List<RootObject> teamsList = new List<RootObject>();
                                                        List<MatchMasterModel> masterList = new List<MatchMasterModel>();
                                                        teamsList = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

                                                        if (teamsList.Count() > 0)
                                                        {
                                                            masterList = teamsList.Select(x => new MatchMasterModel()
                                                            {
                                                                matchDate = x.TimeDateRaw,
                                                                roundno = x.Round,
                                                                fieldno = x.VenueName.Replace("Field", "").Replace("&nbsp;", "").Trim().ToString(),
                                                                divisionName = x.CompName,
                                                                poolName = pool_Name.Replace("Pool", "").Trim().ToString(),
                                                                teamA = x.HomeName,
                                                                versus = "V",
                                                                teamB = x.AwayName,
                                                                halveA = string.Empty,
                                                                halveB = string.Empty
                                                            }).ToList();

                                                            List<IList<Object>> objNewRecords = new List<IList<Object>>();

                                                            IList<Object> obj = new List<Object>();

                                                            if (header == true)
                                                            {
                                                                obj.Add("Time");
                                                                obj.Add("Round");
                                                                obj.Add("Field");
                                                                obj.Add("Division");
                                                                obj.Add("Pool");
                                                                obj.Add("Team");
                                                                obj.Add("v");
                                                                obj.Add("Team");
                                                                obj.Add("Halve 1");
                                                                obj.Add("Halve 2");
                                                                objNewRecords.Add(obj);
                                                            }
                                                            foreach (var a in masterList)
                                                            {
                                                                obj = new List<Object>();
                                                                obj = GenerateData(a, service);
                                                                if (obj != null)
                                                                {
                                                                    objNewRecords.Add(obj);
                                                                }
                                                            }

                                                            string newRange = GetRange(service, sheetName);

                                                            AppendGoogleSheetinBatch(objNewRecords, spreadSheetId, newRange, service);

                                                            if (header == true)
                                                            {
                                                                //The formatHeaderField method will format the header as user wants to.
                                                                formatHeaderField(service, sheetName);
                                                                header = false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void formatHeaderField(SheetsService sheetsService, string sheetName)
        {
            // The ID of the spreadsheet to update.
            // TODO: Update placeholder value.

            BatchUpdateSpreadsheetRequest bussr = new BatchUpdateSpreadsheetRequest();

            //get sheet id by sheet name
            Spreadsheet spr = sheetsService.Spreadsheets.Get(spreadSheetId).Execute();
            Sheet sh = spr.Sheets.Where(s => s.Properties.Title == sheetName).FirstOrDefault();
            int sheetId = (int)sh.Properties.SheetId;

            //create the update request for cells from the first row
            var userEnteredFormat = new CellFormat()
            {
                BackgroundColor = new Color()
                {
                    Blue = (float)0.933,
                    Red = (float)0.393,
                    Green = (float)0.586,
                },
                HorizontalAlignment = "CENTER",
                TextFormat = new TextFormat()
                {
                    FontSize = 24,
                    ForegroundColor = new Color() 
                    {
                        Blue = 1,
                        Red = 1,
                        Green = 1,
                    }
                },
            };

            var updateCellsRequest = new Request()
            {
                RepeatCell = new RepeatCellRequest()
                {
                    Range = new GridRange()
                    {
                        SheetId = sheetId,
                        StartRowIndex = 0,
                        EndRowIndex = 1
                    },
                    Cell = new CellData()
                    {
                        UserEnteredFormat = userEnteredFormat
                    },
                    Fields = "UserEnteredFormat(BackgroundColor,TextFormat)"
                }
            };

            //// TODO: Assign values to desired properties of `requestBody`:
            bussr.Requests = new List<Request>();
            bussr.Requests.Add(updateCellsRequest);

            SpreadsheetsResource.BatchUpdateRequest request = sheetsService.Spreadsheets.BatchUpdate(bussr, spreadSheetId);

            //// To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data.BatchUpdateSpreadsheetResponse response = request.Execute();
            //Data.BatchUpdateSpreadsheetResponse response = await request.ExecuteAsync();

            // TODO: Change code below to process the `response` object:
            //Console.WriteLine(JsonConvert.SerializeObject(response));
        }

        public static string GetResponseFromUrl(string url)
        {
            string response = string.Empty;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                webRequest.Host = webRequest.RequestUri.Host;
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                StreamReader stream = new StreamReader(webResponse.GetResponseStream());
                response = stream.ReadToEnd();
                stream.Dispose();
            }
            catch (Exception ex){}
            return response;
        }

        public static SheetsService CreateGoogleSheets(string sheetName)
        {
            UserCredential credential;
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Google Sheets API Quickstart";
            try
            {
                using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Google Sheets API service.
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                String range = "Class Data!A2:E";

                #region Add sheet into existing spreadsheet
                // Add new Sheet
                //sheetName = sheetName;
                //var addSheetRequest = new AddSheetRequest();
                //addSheetRequest.Properties = new SheetProperties();
                //addSheetRequest.Properties.Title = sheetName;
                //BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
                //batchUpdateSpreadsheetRequest.Requests = new List<Request>();
                //batchUpdateSpreadsheetRequest.Requests.Add(new Request
                //{
                //    AddSheet = addSheetRequest
                //});

                //var batchUpdateRequest =
                //    service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);

                //batchUpdateRequest.Execute();
                #endregion

                SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadSheetId, range);

                return service;

                //This code (Below 4 lines) creates new spreadsheets
                //var myNewSheet = new Google.Apis.Sheets.v4.Data.Spreadsheet();
                //myNewSheet.Properties = new SpreadsheetProperties();
                //myNewSheet.Properties.Title = "Pool Data Sheet";
                //var awsomNewSheet = service.Spreadsheets.Create(myNewSheet).Execute();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected static string GetRange(SheetsService service, string sheetName)
        {
            // Define request parameters.
            String range = sheetName + "!A:A";

            SpreadsheetsResource.ValuesResource.GetRequest getRequest =
                       service.Spreadsheets.Values.Get(spreadSheetId, range);

            ValueRange getResponse = getRequest.Execute();
            IList<IList<Object>> getValues = getResponse.Values;

            int currentCount = 1;
            if (getValues != null)
            {
                currentCount = getValues.Count() + 1; 
            }

            String newRange = "A" + currentCount + ":A";

            return sheetName + "!" + newRange;
        }

        private static IList<Object> GenerateData(MatchMasterModel master, SheetsService service)
        {
            #region Duplicay Removal Section
            string range = GetRange(service, "Sheet_17_04_2018_12_25");
            var request = service.Spreadsheets.Values.Get(spreadSheetId, "Sheet_17_04_2018_12_25!A1:J55");
            
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            //int currentCount = values.Count() + 1;
            int i = 0;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[0] == master.matchDate && row[1] == master.roundno && row[2] == master.fieldno && row[3] == master.divisionName && row[4] == master.poolName && row[5] == master.teamA && row[6] == master.versus && row[7] == master.teamB)
                        i++;
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            #endregion

            IList<Object> obj = new List<Object>();

            if (i == 0)
            {
                obj.Add(master.matchDate);
                obj.Add(master.roundno);
                obj.Add(master.fieldno);
                obj.Add(master.divisionName);
                obj.Add(master.poolName);
                obj.Add(master.teamA);
                obj.Add(master.versus);
                obj.Add(master.teamB);
                obj.Add(master.halveA);
                obj.Add(master.halveB);
            }
            //objNewRecords.Add(obj);
            return obj;
        }

        private static void AppendGoogleSheetinBatch(IList<IList<Object>> values, string spreadsheetId, string newRange, SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.AppendRequest request = service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId, newRange);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var response = request.Execute();
        }

        private static void UpdatGoogleSheetinBatch(IList<IList<Object>> values, string spreadsheetId, string newRange, SheetsService service)
        {
            ValueRange valueRange = new ValueRange();
            valueRange.Values = values;

            SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, newRange);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result2 = update.Execute();
        }

        #region Method to download kml file from Google Map
        public static void GetIBITestMapData()
        {
            try
            {
                string url = "https://www.google.com/maps/d/kml?mid=1t7pGo7Ng-jsfFUwvk9HChVlHcw5vNSAv&forcekml=1";
                
                WebClient wc = new WebClient();
                byte[] hospitaldata = wc.DownloadData(url);

                using (Stream file = File.OpenWrite(System.IO.Path.GetFullPath("IBIMapData.kml")))
                {
                    file.Write(hospitaldata, 0, hospitaldata.Length);
                }
            }
            catch (Exception ex){}
        }
        #endregion

        #region Method to get centroid of Polygon
        public static GeoCoordinateMap GetPolygonCentroid(IList<GeoCoordinateMap> geoCoordinates)
        {
            if (geoCoordinates.Count == 1)
            {
                return geoCoordinates.Single();
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in geoCoordinates)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = geoCoordinates.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new GeoCoordinateMap(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
        }
        #endregion

        #region This method is used to store Field Metrix in sheet
        public static void GetFieldMatrix()
        {
            GetIBITestMapData();
            var ibiMapFile = System.IO.Path.GetFullPath("IBIMapData.kml").ToString();
            string sheetName = "Field Matrix";

            var service = CreateGoogleSheets(sheetName);

            FileStream fs = new FileStream(ibiMapFile, FileMode.Open, FileAccess.Read);

            List<FieldCordinates> fc = new List<FieldCordinates>();
            List<FieldCordinates> fieldCoordRes = new List<FieldCordinates>();
            FieldCordinates fcObj = new FieldCordinates();
            
            try
            {
                // new xdoc instance 
                XmlDocument xDoc = new XmlDocument();

                //load up the xml from the location 
                xDoc.Load(fs);

                // cycle through each child noed
                XmlNodeList xmlnode;
                xmlnode = xDoc.GetElementsByTagName("Folder");
                foreach (XmlNode node in xmlnode)
                {
                    // first node is the url ... have to go to nexted loc node 
                    foreach (XmlNode locNode in node)
                    {
                        // thereare a couple child nodes here so only take data from node named loc 
                        if (locNode.Name == "Placemark")
                        {
                            fcObj = new FieldCordinates();
                            fcObj.FieldName = locNode.FirstChild.InnerText.Trim();
                            // get the content of the loc node 

                            if ((locNode.FirstChild.InnerText.Trim().ToLower()).IndexOf("field") != -1)
                            {
                                foreach (XmlNode childNode in locNode)
                                {
                                    //var name = childNode.GetElementsByTagName("name").ToString();
                                    //var cordinates = coordinates

                                    if (childNode.Name == "Polygon")
                                    {
                                        foreach (XmlNode cn in childNode)
                                        {
                                            foreach (XmlNode ob in cn)
                                            {
                                                foreach (XmlNode lr in ob)
                                                {
                                                    if (lr.LocalName == "coordinates")
                                                    {
                                                        fcObj.Coordinates = lr.InnerText.Trim();
                                                        fc.Add(fcObj);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var fieldObj in fc)
                {
                    fieldObj.latLong = MapFieldWithCoordinates(fieldObj);
                    fieldCoordRes.Add(fieldObj);
                }

                //Method to show header accordition to the dynamic data
                //FieldMatrixHeaderData(service, sheetName, fieldCoordRes);

                //The belowe method calculate distance between two fields using its coordinates
                CalculateDistanceBetweenTwoFields(SortFieldData(fieldCoordRes), service);
            }
            catch (Exception ex) { }
        }
        #endregion

        public static List<GeoCoordinateMap> MapFieldWithCoordinates(FieldCordinates fc)
        {
            string[] lines, coords;
            GeoCoordinateMap geoCoord = new GeoCoordinateMap();
            List<GeoCoordinateMap> geoCoordList = new List<GeoCoordinateMap>();
            try
            {
                lines = fc.Coordinates.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    geoCoord = new GeoCoordinateMap();
                    coords = lines[i].Split(',');
                    geoCoord.Latitude = Convert.ToDouble(coords[1].Trim());
                    geoCoord.Longitude = Convert.ToDouble(coords[0].Trim());
                    geoCoordList.Add(geoCoord);
                }
            }
            catch (Exception ex) { }
            return geoCoordList;
        }

        #region This method calculate distance between two centroids of Polygon
        private static void CalculateDistanceBetweenTwoFields(List<FieldCordinates> fieldCoordRes, SheetsService service)
        {
            int [,] fieldDistance = new int[fieldCoordRes.Count(), fieldCoordRes.Count()];
            string sheetName = "Field Matrix";

            try
            {
                for (int i = 0; i < fieldCoordRes.Count(); i++)
                {
                    var getCentroidA = GetPolygonCentroid(fieldCoordRes[i].latLong);
                    var sCoord = new GeoCoordinate(getCentroidA.p1, getCentroidA.p2);
                    for (int j = 0; j < fieldCoordRes.Count(); j++)
                    {
                        int distance;
                        if (i != j)
                        {
                            if (j > i)
                            {
                                var getCentroidB = GetPolygonCentroid(fieldCoordRes[j].latLong);
                                var eCoord = new GeoCoordinate(getCentroidB.p1, getCentroidB.p2);
                                distance = Convert.ToInt32(sCoord.GetDistanceTo(eCoord));
                                fieldDistance[i, j] = distance;
                                fieldDistance[j, i] = distance;
                            }
                        }
                        else if (i == j)
                        {
                            fieldDistance[i, j] = 0;
                        }
                    }
                }

                #region Update Sheet Data For Field Matrix Sheet
                List<IList<Object>> objNewRecords = new List<IList<Object>>();

                IList<Object> obj = new List<Object>();

                for (int i = 0; i < fieldCoordRes.Count(); i++)
                {
                    obj = new List<Object>();
                    for (int j = 0; j < fieldCoordRes.Count() ; j++ )
                        obj.Add(fieldDistance[i,j]);

                    objNewRecords.Add(obj);

                }

                //string newRange = GetRange(service, sheetName);

                UpdatGoogleSheetinBatch(objNewRecords, spreadSheetId, sheetName + "!B3:AI36", service);
                #endregion

            }
            catch (Exception ex) { }
        }
        #endregion

        private static void FieldMatrixHeaderData(SheetsService sheetsService, string sheetName, List<FieldCordinates> fieldCoordRes)
        {
            // The ID of the spreadsheet to update.
            // TODO: Update placeholder value.

            BatchUpdateSpreadsheetRequest bussr = new BatchUpdateSpreadsheetRequest();

            //get sheet id by sheet name
            Spreadsheet spr = sheetsService.Spreadsheets.Get(spreadSheetId).Execute();
            Sheet sh = spr.Sheets.Where(s => s.Properties.Title == sheetName).FirstOrDefault();
            int sheetId = (int)sh.Properties.SheetId;

            //create the update request for cells from the first row
            var userEnteredFormat = new CellFormat()
            {
                BackgroundColor = new Color()
                {
                    Blue = (float)0.933,
                    Red = (float)0.393,
                    Green = (float)0.586,
                },
                HorizontalAlignment = "CENTER",
                TextFormat = new TextFormat()
                {
                    FontSize = 24,
                    ForegroundColor = new Color()
                    {
                        Blue = 1,
                        Red = 1,
                        Green = 1,
                    }
                },
            };

            var updateCellsRequest = new Request()
            {
                RepeatCell = new RepeatCellRequest()
                {
                    Range = new GridRange()
                    {
                        SheetId = sheetId,
                        StartRowIndex = 0,
                        EndRowIndex = 1
                    },
                    Cell = new CellData()
                    {
                        UserEnteredFormat = userEnteredFormat
                    },
                    Fields = "UserEnteredFormat(BackgroundColor,TextFormat)"
                }
            };

            //// TODO: Assign values to desired properties of `requestBody`:
            bussr.Requests = new List<Request>();
            bussr.Requests.Add(updateCellsRequest);

            SpreadsheetsResource.BatchUpdateRequest request = sheetsService.Spreadsheets.BatchUpdate(bussr, spreadSheetId);

            //// To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data.BatchUpdateSpreadsheetResponse response = request.Execute();
            //Data.BatchUpdateSpreadsheetResponse response = await request.ExecuteAsync();

            // TODO: Change code below to process the `response` object:
            //Console.WriteLine(JsonConvert.SerializeObject(response));
        }

        #region Method that sort FieldCordinates data list in ascending order of Field Name
        private static List<FieldCordinates> SortFieldData(List<FieldCordinates> fieldCoordRes)
        {
            var sortField = fieldCoordRes.Where(x => (x.FieldName.ToLower()).StartsWith("field")).OrderBy(x => x.FieldName).ToList();
            var sortVenueField = fieldCoordRes.Where(x => (x.FieldName.ToLower()).StartsWith("venue")).OrderBy(x => x.FieldName).ToList();

            List<FieldCordinates> SortedFieldCoord = new List<FieldCordinates>();
            FieldCordinates fcObj = new FieldCordinates();

            for (int i = 1; i <= sortField.Count(); i++)
            {
                var sortedFieldData = fieldCoordRes.Where(x => x.FieldName.ToLower() == "field " + i.ToString()).OrderBy(x => x.FieldName).FirstOrDefault();
                sortedFieldData.FieldName = i.ToString();
                SortedFieldCoord.Add(sortedFieldData);
            }

            foreach (var venue in sortVenueField)
            {
                string[] fieldName;
                fieldName = venue.FieldName.Split(' ');
                string fieldNameInitial = fieldName[0].Substring(0, 1) + fieldName[1] + fieldName[3];
                venue.FieldName = fieldNameInitial;
                SortedFieldCoord.Add(venue);
            }
            return SortedFieldCoord;
        }
        #endregion
    }

    public class FieldCordinates
    {
        public string FieldName { get; set; }
        public string Coordinates { get; set; }
        public List<GeoCoordinateMap> latLong { get; set; }
    }
}
