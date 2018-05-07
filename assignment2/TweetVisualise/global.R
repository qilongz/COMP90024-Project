library(tidyverse)
library(magrittr)
library(lubridate)


allLocations <- read_csv("data/location-all.csv")


sentiment4 <- read_csv("data/SentimentFilterWithRegion-SA4.csv", col_types="ccin")
sentiment3 <- read_csv("data/SentimentFilterWithRegion-SA3.csv", col_types="ccin") 
sentiment2 <- read_csv("data/SentimentFilterWithRegion-SA2.csv", col_types="ccin") 
sentiment1 <- read_csv("data/SentimentFilterWithRegion-SA1.csv", col_types="ccin") 



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

sa4@data %<>% rename("Rgn"="sa4_name16")
sa3@data %<>% rename("Rgn"="sa3_name16")
sa2@data %<>% rename("Rgn"="sa2_name16")
sa1@data %<>% mutate("Rgn"="")



genHoverText <- function(rgn,cnt,age,inc, hsize,mortgage)
{
  return (sprintf(
    "<div style='float:left'>
<table width=100%% >
<tr><th colspan=2, class='hdr'>%s</th></tr>
<tr><th>Tweets</th><td>%s</td></tr>
<tr><td colspan=2, class='em'>2016 Census Medians</td></tr>
<tr><td class='sm'>Age (years)</td><td>%s</td></tr>
<tr><td class='sm'>Income (per wk)</td><td>%s</td></tr>
<tr><td class='sm'>Household Size</td><td>%s</td></tr>
<tr><td class='sm'>Mortgage (per mth)</td><td>%s</td></tr>
</table>
</div>
",
    rgn, cnt, age, inc, hsize, mortgage))
}


sa4@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

sa3@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

sa2@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

sa1@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

  

sas <- list(sa1, sa2, sa3, sa4)

# --------------------------------------------------

twitter <- read_delim(  "data\\volumeStats.csv",  ",", col_types = "cDlli", quote = '', progress = FALSE)
tdata <- filter(twitter, YearMth >= ymd(20170701), YearMth <= ymd(20180331))



sentiments <- read_delim("data\\sentimentStats.csv",  ",", col_types = "cDn", quote = '', progress = FALSE)
sdata <- filter(sentiments, YearMth >= ymd(20170701), YearMth <= ymd(20180331))

sentNeturalExcluded <- read_delim("data\\sentimentStatsExcludedNetural.csv",  ",", col_types = "cDn", quote = '', progress = FALSE)
sentNeturalExcluded <- filter(sentNeturalExcluded, YearMth >= ymd(20170701), YearMth <= ymd(20180331))


timeOfDay <- read_delim("data\\sentimentTimeOfDayActivity.csv",  ",", quote = '', progress = FALSE)

sentLocationBanded <- read_delim("data\\sentimentStatsBanded.csv", ",", quote = '', progress = FALSE)
sentTimeOfDayBanded <- read_delim("data\\sentimentStatsHourBanded.csv",  ",", quote = '', progress = FALSE)
sentDayOfWeekBanded <- read_delim("data\\sentimentStatsDayBanded.csv",  ",", quote = '', progress = FALSE)

# recentre pillars
sentLocationBanded %<>% mutate(SentimentBand = SentimentBand - 2.5)
sentTimeOfDayBanded %<>% mutate(SentimentBand = SentimentBand - 2.5)
sentDayOfWeekBanded %<>% mutate(SentimentBand = SentimentBand - 2.5)

dw <- group_by(timeOfDay, DayOfWeek) %>% summarise(Tally = mean(Average))
dw$DayOfWeek <- as.factor(dw$DayOfWeek)
dw$DayOfWeek <- factor(dw$DayOfWeek, levels(dw$DayOfWeek)[c(4, 2, 6, 7, 5, 1, 3)])


#### mobility

toJourneysUsers <- read_csv("data\\geoJourneysUsers.csv",  progress = FALSE)
toJourneysTweets <- read_csv("data\\geoJourneysActivity.csv",  progress = FALSE)
toJourneysAvg <- inner_join(toJourneysUsers, toJourneysTweets, by = c("Source", "X1", "Y1", "X2", "Y2", "Target"))

geoTo <- read_csv("data\\geoToJourneys.csv", progress = FALSE)

region<- c(left=111, right=157, bottom=-46,top=-23)