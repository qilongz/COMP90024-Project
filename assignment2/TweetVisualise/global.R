library(tidyverse)
library(magrittr)
library(lubridate)
library(scales)


allLocations <- read_csv("data/location-all.csv")
recentLocations <- read_csv("data/recentLocations.csv") %>% mutate(Size=6*log(Count))%>% mutate(Size=ifelse(Size<6,6,Size))


sentiment4 <- read_csv("data/SentimentFilterWithRegion-SA4.csv", col_types="ccinn")%>% mutate(Sentiment=Sentiment-39)
sentiment3 <- read_csv("data/SentimentFilterWithRegion-SA3.csv", col_types="ccinn") %>% mutate(Sentiment=Sentiment-39)
sentiment2 <- read_csv("data/SentimentFilterWithRegion-SA2.csv", col_types="ccinn") %>% mutate(Sentiment=Sentiment-39)
sentiment1 <- read_csv("data/SentimentFilterWithRegion-SA1.csv", col_types="ccinn") %>% mutate(Sentiment=Sentiment-39)

recSent4 <- read_csv("data/SentimentRecentWithRegion-SA4.csv", col_types="ccininn") %>% mutate(Sentiment=SumNeutralExc/CountExc-39)
recSent3 <- read_csv("data/SentimentRecentWithRegion-SA3.csv", col_types="ccininn") %>% mutate(Sentiment=SumNeutralExc/CountExc-39)
recSent2 <- read_csv("data/SentimentRecentWithRegion-SA2.csv", col_types="ccininn") %>% mutate(Sentiment=SumNeutralExc/CountExc-39)
recSent1 <- read_csv("data/SentimentRecentWithRegion-SA1.csv", col_types="ccininn") %>% mutate(Sentiment=SumNeutralExc/CountExc-39)

recSent4 <- left_join(recSent4, sentiment4, by=c("RegionId" = "RegionId")) %>%
  mutate(Relative=Sentiment.x - Sentiment.y) %>%
  select(RegionId, Name=Name.x, Observations=Count, Sentiment=Sentiment.x, Relative)
recSent3 <- left_join(recSent3, sentiment3, by=c("RegionId" = "RegionId")) %>%
  mutate(Relative=Sentiment.x - Sentiment.y) %>%
  select(RegionId, Name=Name.x, Observations=Count, Sentiment=Sentiment.x, Relative)
recSent2 <- left_join(recSent2, sentiment2, by=c("RegionId" = "RegionId")) %>%
  mutate(Relative=Sentiment.x - Sentiment.y) %>%
  select(RegionId, Name=Name.x, Observations=Count, Sentiment=Sentiment.x, Relative)
recSent1 <- left_join(recSent1, sentiment1, by=c("RegionId" = "RegionId")) %>%
  mutate(Relative=Sentiment.x - Sentiment.y) %>%
  select(RegionId, Name=Name.x, Observations=Count, Sentiment=Sentiment.x, Relative)


allSent4 <- read_csv("data/SentimentWithRegion-SA4.csv", col_types="ccinn") %>% mutate(Sentiment=Sentiment-39)
allSent3 <- read_csv("data/SentimentWithRegion-SA3.csv", col_types="ccinn") %>% mutate(Sentiment=Sentiment-39)
allSent2 <- read_csv("data/SentimentWithRegion-SA2.csv", col_types="ccinn") %>% mutate(Sentiment=Sentiment-39)
allSent1 <- read_csv("data/SentimentWithRegion-SA1.csv", col_types="ccinn") %>% mutate(Sentiment=Sentiment-39)

# deduce the relative changes between all sentiments & the filtered activity sentiment

sentiment4 <- left_join(sentiment4, allSent4, by=c("RegionId" = "RegionId")) %>%
	mutate(Relative=Sentiment.x - Sentiment.y) %>%
	select(RegionId, Name=Name.x, Observations=Observations.x, Sentiment=Sentiment.x, Relative)
sentiment3 <- left_join(sentiment3, allSent3, by=c("RegionId" = "RegionId")) %>%
	mutate(Relative=Sentiment.x - Sentiment.y) %>%
	select(RegionId, Name=Name.x, Observations=Observations.x, Sentiment=Sentiment.x, Relative)
sentiment2 <- left_join(sentiment2, allSent2, by=c("RegionId" = "RegionId")) %>%
	mutate(Relative=Sentiment.x - Sentiment.y) %>%
	select(RegionId, Name=Name.x, Observations=Observations.x, Sentiment=Sentiment.x, Relative)
sentiment1 <- left_join(sentiment1, allSent1, by=c("RegionId" = "RegionId")) %>%
	mutate(Relative=Sentiment.x - Sentiment.y) %>%
	select(RegionId, Name=Name.x, Observations=Observations.x, Sentiment=Sentiment.x, Relative)


	
fsa4 <- readRDS("data/medians-sa4p02.rds") 
fsa3 <- readRDS("data/medians-sa3p02.rds") 
fsa2 <- readRDS("data/medians-sa2p02.rds")
fsa1 <- readRDS("data/medians-sa1p02.rds")

fsa1@data$sa1_main16<-as.character(fsa1@data$sa1_main16)
fsa1@data%<>% rename("Name"="sa1_main16")

fsa4@data<- left_join(fsa4@data,sentiment4, by=c("sa4_code16" = "RegionId"))
fsa3@data<- left_join(fsa3@data,sentiment3, by=c("sa3_code16" = "RegionId"))
fsa2@data<- left_join(fsa2@data,sentiment2, by=c("sa2_main16" = "RegionId"))
fsa1@data<- left_join(fsa1@data,sentiment1, by=c("Name" = "Name"))

fsa4@data %<>% rename("Rgn"="sa4_name16")
fsa3@data %<>% rename("Rgn"="sa3_name16")
fsa2@data %<>% rename("Rgn"="sa2_name16")
fsa1@data %<>% mutate("Rgn"="")

asa4 <- readRDS("data/medians-sa4p02.rds") 
asa3 <- readRDS("data/medians-sa3p02.rds") 
asa2 <- readRDS("data/medians-sa2p02.rds")
asa1 <- readRDS("data/medians-sa1p02.rds")

asa1@data$sa1_main16<-as.character(asa1@data$sa1_main16)
asa1@data%<>% rename("Name"="sa1_main16")

asa4@data<- left_join(asa4@data,allSent4, by=c("sa4_code16" = "RegionId"))
asa3@data<- left_join(asa3@data,allSent3, by=c("sa3_code16" = "RegionId"))
asa2@data<- left_join(asa2@data,allSent2, by=c("sa2_main16" = "RegionId"))
asa1@data<- left_join(asa1@data,allSent1, by=c("Name" = "Name"))

asa4@data %<>% rename("Rgn"="sa4_name16")
asa3@data %<>% rename("Rgn"="sa3_name16")
asa2@data %<>% rename("Rgn"="sa2_name16")
asa1@data %<>% mutate("Rgn"="")


rsa4 <- readRDS("data/medians-sa4p02.rds") 
rsa3 <- readRDS("data/medians-sa3p02.rds") 
rsa2 <- readRDS("data/medians-sa2p02.rds")
rsa1 <- readRDS("data/medians-sa1p02.rds")

rsa1@data$sa1_main16<-as.character(rsa1@data$sa1_main16)
rsa1@data%<>% rename("Name"="sa1_main16")

rsa4@data<- left_join(rsa4@data,recSent4, by=c("sa4_code16" = "RegionId"))
rsa3@data<- left_join(rsa3@data,recSent3, by=c("sa3_code16" = "RegionId"))
rsa2@data<- left_join(rsa2@data,recSent2, by=c("sa2_main16" = "RegionId"))
rsa1@data<- left_join(rsa1@data,recSent1, by=c("Name" = "Name"))

rsa4@data %<>% rename("Rgn"="sa4_name16")
rsa3@data %<>% rename("Rgn"="sa3_name16")
rsa2@data %<>% rename("Rgn"="sa2_name16")
rsa1@data %<>% mutate("Rgn"="")



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
    rgn, comma(cnt), age, inc, hsize, mortgage))
}


fsa4@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

fsa3@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

fsa2@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

fsa1@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))



asa4@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

asa3@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

asa2@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

asa1@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))
  

fas <- list(fsa1, fsa2, fsa3, fsa4)
aas <- list(asa1, asa2, asa3, asa4)


rsa4@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

rsa3@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

rsa2@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

rsa1@data %<>% mutate(HoverText=genHoverText(
  Rgn, Observations, 
  median_age_persons, median_tot_prsnl_inc_weekly, 
  average_household_size, median_mortgage_repay_monthly
))

ras <- list(rsa1, rsa2, rsa3, rsa4)


# --------------------------------------------------

twitter <- read_delim(  "data/volumeStats.csv",  ",", col_types = "cDlli", quote = '', progress = FALSE)
tdata <- filter(twitter, YearMth >= ymd(20170701), YearMth <= ymd(20180331))



sentiments <- read_delim("data/sentimentStats.csv",  ",", col_types = "cDn", quote = '', progress = FALSE)
sdata <- filter(sentiments, YearMth >= ymd(20170701), YearMth <= ymd(20180331))

sentNeturalExcluded <- read_delim("data/sentimentStatsExcludedNetural.csv",  ",", col_types = "cDn", quote = '', progress = FALSE)
sentNeturalExcluded <- filter(sentNeturalExcluded, YearMth >= ymd(20170701), YearMth <= ymd(20180331))


timeOfDay <- read_delim("data/sentimentTimeOfDayActivity.csv",  ",", quote = '', progress = FALSE)

sentLocationBanded <- read_delim("data/sentimentStatsBanded.csv", ",", quote = '', progress = FALSE)
sentTimeOfDayBanded <- read_delim("data/sentimentStatsHourBanded.csv",  ",", quote = '', progress = FALSE)
sentDayOfWeekBanded <- read_delim("data/sentimentStatsDayBanded.csv",  ",", quote = '', progress = FALSE)

# recentre pillars
sentLocationBanded %<>% mutate(SentimentBand = SentimentBand - 2.5)
sentTimeOfDayBanded %<>% mutate(SentimentBand = SentimentBand - 2.5)
sentDayOfWeekBanded %<>% mutate(SentimentBand = SentimentBand - 2.5)

dw <- group_by(timeOfDay, DayOfWeek) %>% summarise(Tally = mean(Average))
dw$DayOfWeek <- as.factor(dw$DayOfWeek)
dw$DayOfWeek <- factor(dw$DayOfWeek, levels(dw$DayOfWeek)[c(4, 2, 6, 7, 5, 1, 3)])

fullFacet <- read_delim("data/sentimentFullFacet.csv",  ",", quote = '', col_types = "cciinn")
fullFacet$DayOfWeek <- as.factor(fullFacet$DayOfWeek)
fullFacet$DayOfWeek <- factor(fullFacet$DayOfWeek, levels(fullFacet$DayOfWeek)[c(4, 2, 6, 7, 5, 1, 3)])

#### mobility

toJourneysUsers <- read_csv("data/geoJourneysUsers.csv",  progress = FALSE)
toJourneysTweets <- read_csv("data/geoJourneysActivity.csv",  progress = FALSE)
toJourneysAvg <- inner_join(toJourneysUsers, toJourneysTweets, by = c("Source", "X1", "Y1", "X2", "Y2", "Target"))

geoTo <- read_csv("data/geoToJourneys.csv", progress = FALSE)

region<- c(left=111, right=157, bottom=-46,top=-23)