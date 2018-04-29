library(tidyverse)
library(magrittr)

allLocations <- read_csv("data/location-all.csv")

sa4 <- readRDS("data/medians-sa4p02.rds") 
sa4@data %<>% rename("Name"="sa4_name16")
sa3 <- readRDS("data/medians-sa3p02.rds") 
sa3@data %<>% rename("Name"="sa3_name16")
sa2 <- readRDS("data/medians-sa2p02.rds")
sa2@data %<>% rename("Name"="sa2_name16")
sa1 <- readRDS("data/medians-sa1p02.rds")
sa1@data %<>% rename("Name"="sa1_main16")

sas <- list(sa1, sa2, sa3, sa4)