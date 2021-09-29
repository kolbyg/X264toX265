# X264toX265

A small project I created to automate the conversion of various media formats into HEVC.

Automatically pulls all file paths from the sonarr/radarr APIs and sends the files to ffmpeg to be converted.

Use at your own risk.


Instructions:
Run the application normally. On first run it will generate settings.json and exit. Open up the settings file and fill in the details as required.
Simply run the application and it will start converting according to what you have set in settings.json.

Two arguments are supported:
--Force
This will override the maximum conversion limits in settings.json
--ExportList
This will cause a list of all media tagged as "conversion required" to be output in the log. Use this to verify before run
