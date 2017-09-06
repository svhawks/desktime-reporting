# Desktime Reporting
Desktime Reporting project gives insights about employee's working hours using Desktime API.

![](https://raw.githubusercontent.com/svtek/desktime-reporting/master/screenshot_1.png)

### Setup
* Replace `API_KEY` property with your Desktime API Key. You can get your API Key from `https://desktime.com/app/api`
* Replace `MANAGERS` property based on the following example:
    * `manager1:1568,5629,1563,1534,1567,1642,2920,354,305,401,474; manager2:395,156,401`
    * `manager1` and `manager2` are the names of managers and numbers are the ids of employees under these managers.
* Replace `DAILY_WORK_HOURS` property with your company's daily work hours. (Weekends are assumed to be holidays.)

### Deployment
* You can deploy this project to Azure Website.
    * https://msdn.microsoft.com/en-us/library/dd465337(v=vs.110).aspx

### Usage
* Open chrome and type `{YOUR_URL}?manager={MANAGER_NAME}`.

## Idea by
* Sahin https://github.com/sahin

## Developed by
* Selim Fırat Yılmaz https://github.com/mrsfy
* Muhammet https://github.com/muhammet
