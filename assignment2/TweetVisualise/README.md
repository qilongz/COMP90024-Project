
can run directly from the command line with:


%path To R%\R -e "shiny::runApp('%path to directory%\TweetVisualise')"


e.g.

"C:\Program Files\Microsoft\R Open\R-3.4.4\bin\R.exe" -e "shiny::runApp(appDir='.', port=6767, host='0.0.0.0')"





the R instance needs the following packages installed

tidyverse
leaflet
shiny
geojsonio
sp



# there may be other required as well








