using System.Diagnostics.Metrics;
using System.Xml.Linq;

using SortingAlgorithm;


using System;
using System.Net;
using System.Text.Json;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
	int count = 0;
	int list = 0;
	int list1= 0;
	int page = 1;

	string medicalResult = "";
	bool medicalResultBool = false;
	int medicalResultInt = 0;

	private SemaphoreSlim semaphore;
    private List<string> sharedResource;
    private const int MaxConcurrentThreads = 1;

	string textItemList1 = "";
	string textItemList2 = "";
	string textItemList3 = "";

	 private const string FileUrl = "https://rr3---sn-ab5l6nrl.googlevideo.com/videoplayback?expire=1713406519&ei=1y0gZpmOC62IkucPwcCKoAg&ip=170.246.53.5&id=o-ADt79U9IQGK7IL5cBSEnv2vgbDtQsbgCl9zcM-e328jE&itag=18&source=youtube&requiressl=yes&xpc=EgVo2aDSNQ%3D%3D&mh=XP&mm=31%2C26&mn=sn-ab5l6nrl%2Csn-p5qs7nsr&ms=au%2Conr&mv=m&mvi=3&pl=24&initcwndbps=1485000&spc=UWF9f_teW4mDxIpiMlvG-NWoMwcI1mOyyC2G&vprv=1&svpuc=1&mime=video%2Fmp4&cnr=14&ratebypass=yes&dur=219.149&lmt=1666427030750745&mt=1713384532&fvip=3&c=ANDROID&txp=5538434&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Cxpc%2Cspc%2Cvprv%2Csvpuc%2Cmime%2Ccnr%2Cratebypass%2Cdur%2Clmt&sig=AJfQdSswRgIhAJaNu5Ajr-cmf1GG9uVlGHoEbrLhbvhsNyrbTe2s2hIxAiEAvx3XNQ41hd9W4cZ7V6lwEh14MC1tEIpnYP3k-wsJTWs%3D&lsparams=mh%2Cmm%2Cmn%2Cms%2Cmv%2Cmvi%2Cpl%2Cinitcwndbps&lsig=ALClDIEwRgIhAOgBKIEFnm5-7seCAXBAXt63KoTQ6P6pCD8o3J7KtsD6AiEAgsIqIfGl61xDgkUL-JHQiQFOQUxtIcTmk4-86Rn4XNQ%3D&title=%E3%80%90%E6%9D%B1%E6%96%B9%E3%80%91Bad%20Apple!!%20%EF%BC%B0%EF%BC%B6%E3%80%90%E5%BD%B1%E7%B5%B5%E3%80%91"; 

	Networks network;

	Progress<int> progress1 = new Progress<int>();

	public MainPage()
	{
		InitializeComponent();
		CheckAndUpdateTime();

		semaphore = new SemaphoreSlim(MaxConcurrentThreads);
        sharedResource = new List<string>();
	}

	private async Task MyFunction(){
		await Task.Run(() => {
			Thread.Sleep(5000);

			Console.WriteLine("Second Message");
		});
	}

	private void OnAddTaskClicked(object sender, EventArgs e)
	{
		
	}

	private async void DownloadFile_Clicked(object sender, EventArgs e)
	{
		StatusLabel.Text = "Downloading...";
		DownloadProgressBar.Progress = 0;
		DownloadProgressBar.IsVisible = true;

		btnDownload.IsEnabled = false;

		bool success = await DownloadFileAsync(FileUrl, progress =>
		{
			DownloadProgressBar.Progress = progress;
		});

		DownloadProgressBar.IsVisible = false;

		if (success)
		{
			StatusLabel.Text = "Download completed!";
		}
		else
		{
			StatusLabel.Text = "Download failed";
		}
		btnDownload.IsEnabled = true;
	}

	private async Task<bool> DownloadFileAsync(string url, Action<double> progressCallback)
	{
		try
		{
			using (var client = new WebClient())
			{
				client.DownloadProgressChanged += (sender, e) =>
				{
					double progressPercentage = (double)e.BytesReceived / e.TotalBytesToReceive;
					progressCallback?.Invoke(progressPercentage);
				};

				await client.DownloadFileTaskAsync(new Uri(url), "BadApple.mp4");
				return true;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Download error: {ex.Message}");
			return false;
		}
	}

	private async void CheckAndUpdateTime()
    {
        while (true)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://worldtimeapi.org/api/timezone/");
                    HttpResponseMessage response = await client.GetAsync("Pacific/Honolulu");
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic timezoneData = JsonSerializer.Deserialize<TimeZoneResponse>(jsonResponse);
                        DateTime currentTime = DateTime.Parse(timezoneData.utc_datetime);
                        DateTime futureTime = currentTime.AddHours(5).AddMinutes(30).AddSeconds(15);
						

						await semaphore.WaitAsync(); // Wait to acquire the semaphore
						try
						{
							// Simulate adding an item to a shared resource (e.g., list)
							//string newItem = DateTime.Now.ToString("HH:mm:ss");
							sharedResource.Add(futureTime.ToString());
							UpdateList();
						}
						finally
						{
							//Console.WriteLine("Release Taimu:" + sharedResource.Count);
							semaphore.Release(); // Release the semaphore
						}

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            currentTimeLabel.Text = $"Current Time: {currentTime}";
                            futureTimeLabel.Text = $"Future Time: {futureTime}";
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            currentTimeLabel.Text = "Failed to get Maui time zone.";
                            futureTimeLabel.Text = string.Empty;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    currentTimeLabel.Text = $"Error: {ex.Message}";
                    futureTimeLabel.Text = string.Empty;
                });
            }

            //await Task.Delay(TimeSpan.FromMinutes(0.1)); // Check every 30 minutes
        }
    }
	public class TimeZoneResponse
    {
        public string utc_datetime { get; set; }
    }

	private async void StartHeavyProcessButton_Clicked(object sender, EventArgs e)
	{//function
		StartHeavyProcessButton.IsEnabled = false;
		progressBarString.Progress = 0.0; // Reset progress before starting

		try
		{//
			await Task.Run(async () =>{//
				for (int i = 0; i < 20; i++){//
					using (var client = new HttpClient())
					{
						HttpResponseMessage response = await client.GetAsync("https://perdiviretesting.000webhostapp.com/BaseDatos.php?nombre_de_parte=Cerebro&columna=anatomia_basica");
						if (response.IsSuccessStatusCode) {
							string response2 = await response.Content.ReadAsStringAsync();

							if (medicalResultBool == false){
								Console.WriteLine("Medical Result: " + medicalResult);
								Console.WriteLine("Response 2: " + response2);
								medicalResultBool = true;
								medicalResult = response2;
							} else {
								if (response2 != medicalResult){
									medicalResult = response2;
									medicalResultInt = medicalResultInt + 1;
									Console.WriteLine("The information has changed "+ medicalResultInt + "times");
									Device.BeginInvokeOnMainThread(() =>
									{
										Alert.Text = "The information has changed "+ medicalResultInt + "times";
									});
									
									
								}
							}

							response2 = ReverseString(response2);
							double progressValue = (double)i / 100;
							Device.BeginInvokeOnMainThread(() => progressBarString.Progress = progressValue);
							Task.Delay(10000).Wait(); // Adjust delay as needed
							Device.BeginInvokeOnMainThread(() =>
							{
								IterationLabel.Text = $"Result: {i} {response2}";
							});//device
							Console.WriteLine(medicalResult);
							
						}//if
						else
						{//
							Console.WriteLine("Unable to connect to the server");
						}//else
					}//using
					
				}//for
				
			});//task
			
		}//try
		catch (Exception ex)
		{//catch
			Device.BeginInvokeOnMainThread(() =>
			{
				Console.WriteLine("Exception on string process: " + ex.Message);
			});
		}//catch

		StartHeavyProcessButton.IsEnabled = true;
	}//function


	private string ReverseString(string input)
	{
		char[] charArray = input.ToCharArray();
		Array.Reverse(charArray);
		return new string(charArray);
	}

	private void UpdateIterationLabel(int iteration)
	{
		Device.BeginInvokeOnMainThread(() =>
		{
			IterationLabel.Text = $"Iteration: {iteration}";
		});
	}

	private async void AddItemButton_Clicked(object sender, EventArgs e)
	{
		await semaphore.WaitAsync(); // Wait to acquire the semaphore

		try
		{
			// Simulate adding an item to a shared resource (e.g., list)
			string newItem = DateTime.Now.ToString("HH:mm:ss");
			sharedResource.Add(newItem);
			UpdateList();
		}
		finally
		{
			Console.WriteLine("Release:" + sharedResource.Count);

			await Task.Delay(TimeSpan.FromMinutes(0.5));

			semaphore.Release(); // Release the semaphore
		}
	}

	private void UpdateList()
	{
		string output = "";
		if (textItemList1 == ""){
			textItemList1 = sharedResource.Last();
		} else if (textItemList2 == ""){
			textItemList2 = sharedResource.Last();
		}else if (textItemList3 == ""){
			textItemList3 = sharedResource.Last();
		} else{
			textItemList1 = textItemList2;
			textItemList2 = textItemList3;
			textItemList3 = sharedResource.Last();
		}

		output = textItemList1 + "\n" + textItemList2 + "\n" + textItemList3;
		
		
		Device.BeginInvokeOnMainThread(() =>
		{
			ItemsList.Text = output; // Update the UI list
		});
	}
    
}

