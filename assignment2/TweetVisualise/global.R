library(tidyverse)
library(magrittr)

# ----------------------------------------------------------------------------------------
# load base data 

allLocations <- read_csv("data/location-all.csv")

sentiment4 <- read_csv("data/SentimentWithRegion-SA4.csv") %>% mutate(RegionId=factor(RegionId))
sentiment3 <- read_csv("data/SentimentWithRegion-SA3.csv") %>% mutate(RegionId=factor(RegionId))
sentiment2 <- read_csv("data/SentimentWithRegion-SA2.csv") %>% mutate(RegionId=factor(RegionId))
sentiment1 <- read_csv("data/SentimentWithRegion-SA1.csv", col_types = "icin") 


sa4 <- readRDS("data/medians-sa4p02.rds") 
sa3 <- readRDS("data/medians-sa3p02.rds") 
sa2 <- readRDS("data/medians-sa2p02.rds")
sa1 <- readRDS("data/medians-sa1p02.rds")
sa1@data$sa1_main16<-as.character(sa1@data$sa1_main16)
sa1@data%<>% rename("Name"="sa1_main16")


sa4@data<- left_join(sa4@data,sentiment4, by=c("sa4_code16" = "RegionId"))
sa3@data<- left_join(sa3@data,sentiment3, by=c("sa3_code16" = "RegionId"))
sa2@data<- left_join(sa2@data,sentiment2, by=c("sa2_main16" = "RegionId"))
sa1@data<- left_join(sa1@data,sentiment1, by=c("Name" = "Name"))

sas <- list(sa1, sa2, sa3, sa4)

# ----------------------------------------------------------------------------------------

source("Activity.R")
source("Sentiment.R")
source("Mobility.R")