#
#  transform data 
#

#    "C:\Program Files\Microsoft\R Open\R-3.4.4\bin\R.exe"  --no-save  --max-ppsize 100000  < dataPrepare.R  

library(tidyverse)
library(geojsonio)
library(rmapshaper)

for (area in c(4,3,2)) {

  name <- paste("data/medians-sa", area, "p02.geojson", sep='')
  oname <- paste("data/medians-sa", area, "p02.rds", sep='')
  
  message(name)
  
  sa <- geojsonio::geojson_read(name, what="sp")
  sas <- rmapshaper::ms_simplify(sa)
  saveRDS(sas, oname)
}

