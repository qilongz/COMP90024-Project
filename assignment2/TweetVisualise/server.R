library(leaflet)
library(RColorBrewer)
library(scales)

library(lattice)
library(tidyverse)
library(lubridate)
library(ggmap)
library(htmltools)


function(input, output, session) {
  
  
  
  
  
  
  
  
  # -------------------------------------------------------------------------
  # activity plots
  
  output$plotActivityAllVolByMth <- renderPlot({
    allCityTtls <- tdata %>%
      group_by(YearMth) %>%
      summarise(Tally = sum(Count)) %>%
      mutate(Location = "Total") %>%
      select(Location, YearMth, Tally)
    
    byCityTtls <- tdata %>%
      group_by(Location, YearMth) %>%
      summarise(Tally = sum(Count)) %>%
      ungroup()
    
    # byCityTtls<-bind_rows(byCityTtls, allCityTtls)
    
    ggplot(data = byCityTtls,
           aes(
             x = YearMth,
             y = Tally,
             group = Location,
             colour = Location
           )) +
      geom_line() +
      geom_point() +
      geom_line(
        data = allCityTtls,
        aes(
          x = YearMth,
          y = Tally,
          group = Location,
          colour = Location
        ),
        size = 1,
        linetype = "longdash"
      ) +
      xlab('Month Year') +
      ylab('Tweet Tally') +
      scale_color_brewer(palette = 'Dark2') +
      scale_y_continuous(labels = comma) +
      scale_x_date(date_breaks = "1 month", date_labels = "%b %y") +
      labs(title = "Volume of geo-located tweets by month") +
      theme(text = element_text(size = 18))
    
  })
  
  output$plotActivityNewVolByMth <- renderPlot({
    allCityTtls <- filter(tdata, HasGeo) %>%
      group_by(YearMth) %>%
      summarise(Tally = sum(Count)) %>%
      mutate(Location = "Total") %>%
      select(Location, YearMth, Tally)
    
    byCityTtls <- filter(tdata, HasGeo) %>%
      group_by(Location, YearMth) %>%
      summarise(Tally = sum(Count)) %>%
      ungroup()
    
    # byCityTtls<-bind_rows(byCityTtls, allCityTtls)
    
    ggplot(data = byCityTtls,
           aes(
             x = YearMth,
             y = Tally,
             group = Location,
             colour = Location
           )) +
      geom_line() +
      geom_point() +
      geom_line(
        data = allCityTtls,
        aes(
          x = YearMth,
          y = Tally,
          group = Location,
          colour = Location
        ),
        size = 1,
        linetype = "longdash"
      ) +
      scale_color_brewer(palette = 'Dark2') +
      xlab('Month Year') +
      ylab('Tweet Tally') +
      scale_y_continuous(labels = comma) +
      scale_x_date(date_breaks = "1 month", date_labels = "%b %y") +
      labs(title = "Volume of tweets with exact locations (GPS coordinates) by month") +
      theme(text = element_text(size = 18))
    
  })
  
  
  # -------------------------------------------------------------------------
  # sentiment plots
  
  # By Location
  
  output$plotAverageSentimentByMthByLocation <- renderPlot({
    lallCityTtls <- sdata %>%
      group_by(YearMth) %>%
      summarise(Tally = mean(AverageSentiment)) %>%
      mutate(Location = " mean") %>%
      select(Location, YearMth, Tally)
    
    lbyCityTtls <- sdata %>%
      group_by(Location, YearMth) %>%
      summarise(Tally = mean(AverageSentiment)) %>%
      ungroup()
    
    ggplot(data = lbyCityTtls,
           aes(
             x = YearMth,
             y = Tally,
             group = Location,
             colour = Location
           )) +
      geom_line() +
      geom_point() +
      geom_smooth(
        method = 'loess',
        span = 0.3,
        data = lallCityTtls,
        aes(
          x = YearMth,
          y = Tally,
          group = Location,
          colour = Location
        ),
        size = 1,
        linetype = "longdash"
      ) +
      xlab('Month Year') +
      ylab('Average Sentiment') +
      scale_color_brewer(palette = 'Dark2') +
      scale_y_continuous(labels = percent) +
      scale_x_date(date_breaks = "1 month", date_labels = "%b %y") +
      labs(title = "Average VADER sentiment of geo-located tweets by month") +
      theme(text = element_text(size = 16))
    
  })
  
  
  output$plotDistributionSentimentAll <- renderPlot({
    ggplot(data = sentLocationBanded,
           aes(
             x = SentimentBand,
             y = Count,
             group = Location,
             color = Location
           )) +
      geom_line() +
      scale_color_brewer(palette = 'Dark2') +
      scale_y_continuous('Volume', labels = comma) +
      labs(title = "Tweet sentiment distribution by location") +
      theme(text = element_text(size = 16))
    
  })
  
  output$plotDistributionSentimentAllNormalised <- renderPlot({
    sentLocationBandedTtls <-
      filter(sentLocationBanded, SentimentBand < -5 |
               5 < SentimentBand) %>%
      group_by(SentimentBand) %>%
      summarise(AllCount = sum(Count), AllTotal = sum(Total)) %>%
      mutate(Location = "Total")
    
    
    filter(sentLocationBanded, SentimentBand < -5 |
             5 < SentimentBand) %>%
      ggplot(aes(
        x = SentimentBand,
        y = Count / Total,
        group = Location,
        color = Location
      )) +
      geom_line() +
      geom_smooth(
        method = 'loess',
        se = FALSE,
        span = 0.3,
        data = sentLocationBandedTtls,
        aes(
          x = SentimentBand,
          y = AllCount / AllTotal,
          group = Location,
          colour = Location
        ),
        size = 1,
        linetype = "longdash"
      ) +
      scale_y_continuous('Scaled Volume', labels = percent) +
      scale_color_brewer(palette = 'Dark2') +
      xlab("Sentiment (percent)") +
      labs(title = "Tweet sentiment distribution by location") +
      labs(subtitle = "Excluding neutral tweets (-%5 < sentiment < 5%)")    +
      theme(text = element_text(size = 16))
    
  })
  
  # By Time of Day
  
  output$plotAverageSentimentByTimeOfDay <- renderPlot({
    group_by(timeOfDay, Hour) %>%
      summarise(Tally = mean(Average)) %>%
     
      ggplot(aes(x = as.integer(Hour), y = Tally)) +
      geom_line(color = 'red4', size = 1) +
      xlab('Hour of Day (LocalTime)') +
      ylab('Average VADER sentiment in hour [h,h+1) of day') +
      scale_y_continuous(labels = percent, limits = c(0.11 , 0.14)) +
      labs(title = "Average variation in tweet sentiment with time of day" )    +
      theme(text = element_text(size = 16))
  })
  
  output$plotAverageSentimentByFilteredTimeOfDay <- renderPlot({
    sentTimeOfDayBandedTtls <- filter(sentTimeOfDayBanded, SentimentBand < -5 | 5 < SentimentBand) %>%
      group_by(SentimentBand) %>%
      summarise(AllCount = sum(Count), AllTotal = sum(Total)) %>%
      mutate(Hour = "Whole Day")
    
    filter(sentTimeOfDayBanded, SentimentBand < -5 | 5 < SentimentBand) %>%
      ggplot(aes(x = SentimentBand, y = Count / Total, group = Hour, color = Hour)) +
      geom_line() +
      geom_smooth(method = 'loess', se = FALSE, span = 0.3,
                  data = sentTimeOfDayBandedTtls,
                  aes(x = SentimentBand, y = AllCount / AllTotal, group = Hour, colour = Hour),
                  size = 1.5, linetype = "longdash") +
      scale_x_continuous('Sentiment (percent)') +
      scale_y_continuous('Scaled Volume', labels = percent) +    
      labs(title = "Tweet sentiment distribution by Time of Day") +
      labs(subtitle = "Excluding neutral tweets (-%5 < sentiment < 5%)")    +
      theme(text = element_text(size = 16))
  })
  
  
  # By Day of Week
  
  
  output$plotAverageSentimentByDayofWeek <- renderPlot({
    
    ggplot(data = dw, aes(x = as.integer(DayOfWeek), y = Tally)) +
      geom_line(color = 'red4', size = 1) +
      scale_x_continuous(
        'Day of Week (LocalTime)',
        breaks = c(1, 2, 3, 4, 5, 6, 7),
        labels = c(
          'Sunday',
          'Monday',
          'Tuesday',
          'Wednesday',
          'Thursday',
          'Friday',
          'Saturday'
        )
      ) +
      scale_y_continuous(
        'Average VADER sentiment for day of week',
        labels = percent,
        limits = c(0.11, 0.14)
      ) +
      labs(title = "Average variation tweet sentiment with day of week")    +
      theme(text = element_text(size = 16))
  })
  
  output$plotAverageSentimentByFilteredDayOfWeek <- renderPlot({
    sentDayOfWeekBandedTtls <-
      filter(sentDayOfWeekBanded, SentimentBand < -5 |
               5 < SentimentBand) %>%
      group_by(SentimentBand) %>%
      summarise(AllCount = sum(Count), AllTotal = sum(Total)) %>%
      mutate(DayOfWeek = "Cross Whole Day")
    
    filter(sentDayOfWeekBanded, SentimentBand < -5 |
             5 < SentimentBand) %>%
      ggplot(aes(
        x = SentimentBand,
        y = Count / Total,
        group = DayOfWeek,
        color = DayOfWeek
      )) +
      geom_line() +
      geom_smooth(
        method = 'loess',
        se = FALSE,
        span = 0.3,
        data = sentDayOfWeekBandedTtls,
        aes(
          x = SentimentBand,
          y = AllCount / AllTotal,
          group = DayOfWeek,
          colour = DayOfWeek
        ),
        size = 1.5,
        linetype = "longdash"
      ) +
      scale_x_continuous('Sentiment (percent)') +
      scale_y_continuous('Scaled Volume', labels = percent) +
      labs(title = "Tweet sentiment distribution by time of day") +
      labs(subtitle = "Excluding neutral tweets (-%5 < sentiment < 5%)")   +
      theme(text = element_text(size = 16))
  })
  
  
  
  # Based lined
  
  output$plotBaseByLocation <- renderPlot({
    
    allCityTtls <- sentNeturalExcluded %>%
      group_by(YearMth) %>%
      summarise(Tally = mean(AverageSentiment)) %>%
      mutate(Location = " mean") %>%
      select(Location,YearMth,Tally)
    
    byCityTtls <- sentNeturalExcluded %>%
      group_by(Location, YearMth) %>%
      summarise(Tally = mean(AverageSentiment)) %>%
      ungroup()
    
    allAvg <- mean(byCityTtls$Tally)
    
    
    ggplot(data = byCityTtls, aes(x = YearMth, y = Tally - allAvg, group = Location, fill = Location)) +
      geom_col(position = "dodge", color='white') +
      xlab('Month Year') +
      ylab('Average Sentiment') +
      scale_color_brewer(palette = 'Paired') +
      scale_y_continuous(labels = percent) +
      scale_x_date(date_breaks = "1 month", date_labels = "%b %y") +
      labs(title = "Baselined average VADER sentiment of geo-located tweets by month") +
      labs(subtitle = "Excluding neutral tweets (-%5 < sentiment < 5%)")    +
      theme(text = element_text(size = 16))
  })
  
  
  output$plotBaseByHour <- renderPlot({
    
    byHour <- group_by(timeOfDay, Hour) %>%
      summarise(Tally = mean(Average))
    byHourAvg <- mean(byHour$Tally)
    
    
    ggplot(data = byHour, aes(x = as.integer(Hour), y = Tally - byHourAvg)) +
      geom_col(fill = 'red4') +
      xlab('Hour of Day (LocalTime)') +
      ylab('Average VADER sentiment in hour [h,h+1) of day') +
      scale_y_continuous(labels = percent ) +    
      labs(title="Baselined average variation in tweet sentiment with time of day")+
      labs(subtitle = "Excluding neutral tweets (-%5 < sentiment < 5%)")    +
    theme(text = element_text(size = 16))
  })
  
    output$plotBaseByDay <- renderPlot({
      
      ggplot(data = dw, aes(x = as.integer(DayOfWeek), y = Tally - mean(dw$Tally))) +
        geom_col(fill = 'red4') +
        scale_x_continuous('Day of Week (LocalTime)',breaks = c(1,2,3,4,5,6,7),
                           labels=c('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday')) +   
        scale_y_continuous('Average VADER sentiment for day of week',labels = percent) +
        labs(title="Baselined average variation tweet sentiment with day of week") +
        labs(subtitle = "Excluding neutral tweets (-%5 < sentiment < 5%)") +
      theme(text = element_text(size = 16))
    })
    
    
    
    output$plotDayLocationFacet <- renderPlot({
      
      mid<-sum(fullFacet$Sum)/sum(fullFacet$Count)
      
      locationDay <- fullFacet %>%
        group_by(Location,DayOfWeek) %>%
        summarise(Tally = sum(Sum)/sum(Count)-mid)
      
      ggplot(data = locationDay, aes(x = DayOfWeek, y = Tally/100)) +
        geom_col(fill = 'firebrick2') +
        xlab('Day Of Week') +
        ylab('Relative Sentiment') +
        scale_y_continuous(labels = percent ) +    
        facet_grid(Location ~ .) +
        theme(text = element_text(size = 16))
    })
    
    
    output$plotLocationFullFacet <- renderPlot({
	
     mid<-sum(fullFacet$Sum)/sum(fullFacet$Count)/100      
     
      ggplot(data = fullFacet, aes(x = TimeOfDay, y = Sum/Count/100-mid)) +
        geom_col(fill = 'firebrick3') +
        xlab('Hour of Day') +
        ylab('Relative Sentiment') +
        scale_y_continuous(labels = percent ) +    
       facet_grid(Location ~ DayOfWeek) +
        theme(text = element_text(size = 16))
    })
    
    output$plotDayOfWeekFullFacet <- renderPlot({
	
      mid<-sum(fullFacet$Sum)/sum(fullFacet$Count)/100      
      
      ggplot(data = fullFacet, aes(x = TimeOfDay, y = Sum/Count/100-mid)) +
        geom_col(fill = 'firebrick4') +
        xlab('Hour of Day') +
        ylab('Relative Sentiment') +
        scale_y_continuous(labels = percent ) +    
        facet_grid(DayOfWeek ~ Location ) +
        theme(text = element_text(size = 16))
    })
    
    
    
  ## Mobility  -------------------------------------------------
    
    # Numbers
    
    output$plotNumFromMelb <- renderPlot({
      
      srcCity <- filter(toJourneysUsers, Source == "melbourne")
      
      ggmap(get_stamenmap(region, zoom = 4, maptype = "toner-lite")) +
        geom_segment(data = srcCity,
                     aes(x = X1, y = Y1, xend = X2, yend = Y2, size = Users),
                     arrow = arrow(angle = 20), lineend = 'square', color = 'deeppink4', alpha = 0.5) +
        geom_point(data = srcCity, aes(x = X1, y = Y1),
                   size = 4, color = 'deeppink4') +
        xlab('Longitude') +
        ylab('Latitude') +
        labs(title = 'Number of Melbourne residents who have tweeted in other cities') +
        labs(caption = 'Line width is proportional to number of tweeters who visit linked city')  +
        theme(text = element_text(size = 16))
    })
    
    
    output$plotNumToMelb <- renderPlot({
      
      srcCity <- filter(toJourneysUsers, Target == "melbourne")
      
      ggmap(get_stamenmap(region, zoom = 4, maptype = "toner-lite")) +
        geom_segment(data = srcCity,
                     aes(x = X1, y = Y1, xend = X2, yend = Y2, size = Users),
                     arrow = arrow(angle = 13), lineend = 'square', color = 'green4', alpha = 0.5) +
        geom_point(data = srcCity, aes(x = X1, y = Y1),
                   size = 4, color = 'green4') +
        xlab('Longitude') +
        ylab('Latitude') +
        labs(title = 'Number of non-Melbourne residents who tweeted when visiting Melbourne') +
        labs(caption = 'Line width is proportional to number of tweeters who visit linked city')   +
        theme(text = element_text(size = 16))
    })
    
    
    # Volume
    
    output$plotVolFromMelb <- renderPlot({
      
      srcCity <- filter(toJourneysAvg, Source == "melbourne")
      
      ggmap(get_stamenmap(region, zoom = 4, maptype = "toner-lite")) +
        geom_segment(data = srcCity,
                     aes(x = X1, y = Y1, xend = X2, yend = Y2, size = Tweets/Users),
                     arrow = arrow(angle = 20), lineend = 'square', color = 'deeppink4', alpha = 0.5) +
        geom_point(data = srcCity, aes(x = X1, y = Y1),
                   size = 4, color = 'deeppink4') +
        xlab('Longitude') +
        ylab('Latitude') +
        labs(title = 'Mean tweets by Melbourne residents who tweeted while in other cities') +
        labs(caption = 'Line width is proportional to average tweets')  +
        theme(text = element_text(size = 16))
    })
    
    
    output$plotVolToMelb <- renderPlot({
      
      srcCity <- filter(toJourneysAvg, Target == "melbourne")
      
      ggmap(get_stamenmap(region, zoom = 4, maptype = "toner-lite")) +
        geom_segment(data = srcCity,
                     aes(x = X1, y = Y1, xend = X2, yend = Y2, size = Tweets/Users),
                     arrow = arrow(angle = 13), lineend = 'square', color = 'green4', alpha=0.5) +
        geom_point(data = srcCity, aes(x = X1, y = Y1),
                   size = 4, color = 'green4') +
        xlab('Longitude') +
        ylab('Latitude') +
        labs(title = 'Mean tweets by non-Melbourne residents who tweeted when visiting Melbourne') +
        labs(caption = 'Line width is proportional to average tweets')   +
        theme(text = element_text(size = 16))
    })
    
    
    # sample locations 
    
    output$mapInnerMelbDest <- renderPlot({
      
      region <- c(left = 144.91, right = 145.0, top = -37.78, bottom = -37.875)
      innermelb <- filter(geoTo,
                          region['left'] <= X2 & X2 <= region['right'],
                          region['top'] >= Y2 & Y2 >= region['bottom'])
      
      ggmap(get_stamenmap(region, zoom = 14, maptype = "toner-lite")) +
        stat_density_2d(data = innermelb, aes(x = X2, y = Y2, fill = ..level..), geom = "polygon", alpha = 0.5) +
        scale_fill_distiller(palette = "Spectral") +
        geom_point(data = innermelb, aes(x = X2, y = Y2), color = 'navy') +
        scale_alpha(trans = 'log10') + xlab('Longitude') + ylab('Latitude') +
        theme(legend.position = "none")
    })
    
    
    output$mapInnerSydDest <- renderPlot({
      
      region <- c(left = 151.185, right = 151.23, top = -33.842, bottom = -33.875)
      innersyd <- filter(geoTo,
                         region['left'] <= X2 & X2 <= region['right'],
                         region['top'] >= Y2 & Y2 >= region['bottom'])
      
      ggmap(get_stamenmap(region, zoom = 15, maptype = "toner-lite")) +
        stat_density_2d(data = innersyd, aes(x = X2, y = Y2, fill = ..level..), geom = "polygon", alpha = 0.5) +
        scale_fill_distiller(palette = "Spectral") +
        geom_point(data = innersyd, aes(x = X2, y = Y2), color = 'navy') +
        scale_alpha(trans = 'log10') + xlab('Longitude') + ylab('Latitude') +
        theme(legend.position = "none")
    })
    
    
  
  ## Interactive Map -------------------------------------------------
  
  # Create the [map: Activity Distribution]
  output$mapExploreDistribution <- renderLeaflet({
    leaflet(options = leafletOptions()) %>%
      addTiles(urlTemplate = "//{s}.tiles.mapbox.com/v3/jcheng.map-5ebohr46/{z}/{x}/{y}.png",
               attribution = 'Maps by <a href="http://www.mapbox.com/">Mapbox</a>') %>%
      fitBounds(114, -11, 154, -44)
    
  })
  
  mapExploreDistributionZoomState <- reactive({
    zoom <- 4
    bounds <- list(
      west = 114,
      east = 154,
      north = 11,
      south = -44
    )
    
    if (!is.null(input$mapExploreDistribution_bounds)) {
      bounds <- input$mapExploreDistribution_bounds
      zoom <- input$mapExploreDistribution_zoom
    }
    
    list("zoom" = zoom, "bounds" = bounds)
  })
  
  
  observe({
    zs <- mapExploreDistributionZoomState()
    
    if (exists("zs") & !is.null(zs)) {
      precision <- if_else(zs$zoom <= 6, 1, zs$zoom / 4)
      
      activeLocations <- allLocations %>%
        filter(
          zs$bounds$west <= Xloc & Xloc <= zs$bounds$east,
          zs$bounds$north >= Yloc &
            Yloc >= zs$bounds$south
        ) %>%
        mutate(Xloc = round(Xloc, precision),
               Yloc = round(Yloc, precision)) %>%
        group_by(Xloc, Yloc) %>%
        summarise(Size = log10(sum(Count)) * 3) %>%
        ungroup()
      
      
      leafletProxy("mapExploreDistribution", data = activeLocations) %>%
        clearMarkers() %>%
        addCircleMarkers(
          lng =  ~ Xloc,
          lat =  ~ Yloc,
          radius = ~ Size,
          stroke = FALSE,
          fillOpacity = 0.75,
          color = 'red'
        )
      
    }
  })
  
    
    #              -------------------------------------
    
    # Create the [map: Sentiment Analysis]
    output$mapSentimentByArea <- renderLeaflet({
      aid <- 4
	  bid <- 2
      pal <- colorNumeric(c( "navy","white","red" ),NULL)
      
      sd <- fas[[aid]]
      if (bid ==1) {
        sd@data %<>% mutate(Sentiment=coalesce(Relative,0))
      }
      
      leaflet(sd) %>%
        addTiles(urlTemplate = "//{s}.tiles.mapbox.com/v3/jcheng.map-5ebohr46/{z}/{x}/{y}.png",
                 attribution = 'Maps by <a href="http://www.mapbox.com/">Mapbox</a>') %>%
        fitBounds(114,-11, 154,-44)   %>%
        addPolygons(
          stroke = TRUE,
          smoothFactor = 1,
          fillOpacity = 0.5,
          weight = 1,
          opacity = 0.6,
          color = 'black',
          fillColor = ~ pal(Sentiment),
          highlightOptions = highlightOptions(
            color = 'red',
            weight = 3,
            bringToFront = TRUE
          )
        ) %>%
        addLegend(
          pal = pal,
          title = 'Sentiment',
          values = ~ Sentiment,
          opacity = 0.75
        )
    })
    
    mapSentimentByAreaZoomState <- reactive({
      zoom <- 4
      bounds <- list(
        west = 114,
        east = 154,
        north = 11,
        south = -44
      )
      
      if (!is.null(input$mapSentimentByArea_bounds)) {
        bounds <- input$mapSentimentByArea_bounds
        zoom <- input$mapSentimentByArea_zoom
      }
      
      list("zoom" = zoom, "bounds" = bounds)
    })
    
    observe({
      zs <- mapSentimentByAreaZoomState()
      
      if (exists("zs") & !is.null(zs)) {
        aid <- as.integer(input$SentimentBySA.areaId)
		    bid <- as.integer(input$SentimentBySA.baseId)
		
        sd <- fas[[aid]]
		    if (bid ==1) {
			    sd@data %<>% mutate(Sentiment=coalesce(Relative,0))
		    }
        
        if (!isTRUE(session$userData$mapSentimentByArea) &
            zs$zoom > 12) {
          session$userData$mapSentimentByArea <- TRUE
          updateRadioButtons(
            session,
            "SentimentBySA.areaId",
            label = "Area:",
            choices = c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2,
              "SA1" = 1
            ),
            selected = aid
          )
        }
        
        if (isTRUE(session$userData$mapSentimentByArea) &
            zs$zoom <= 12) {
          session$userData$mapSentimentByArea <- FALSE
          updateRadioButtons(
            session,
            "SentimentBySA.areaId",
            label = "Area:",
            choices = c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2
            ),
            selected = 2
          )
        }
        
        bb <- zs$bounds
        ff <-        ((sd$bbox1 <= bb$west &
                         bb$west <= sd$bbox3) | (sd$bbox1 <= bb$east &
                                                   bb$east <= sd$bbox3) |
                        (sd$bbox1 >= bb$west &
                           bb$east >= sd$bbox1) |
                        (sd$bbox3 >= bb$west &
                           bb$east >= sd$bbox3)
        ) &
          ((sd$bbox2 <= bb$south &
              bb$south <= sd$bbox4) |
             (sd$bbox2 <= bb$north & bb$north <= sd$bbox4) |
             (sd$bbox2 >= bb$south &
                bb$north >= sd$bbox2) |
             (sd$bbox4 >= bb$south & bb$north >= sd$bbox4)
          )
        
        srcData <- sd[ff,]
        
        if (!is.null(srcData)) {
          pal <- colorNumeric(c( "navy","white","red" ), NULL)
          
          leafletProxy("mapSentimentByArea", data = srcData) %>%
            clearShapes() %>%
            clearControls() %>%
            addPolygons(
              stroke = TRUE,
              smoothFactor = 1,
              fillOpacity = 0.5,
              weight = 1,
              opacity = 0.6,
              color = 'black',
              fillColor = ~ pal(Sentiment),
              highlightOptions = highlightOptions(
                color = 'red',
                weight = 3,
                bringToFront = TRUE
              ),
              label = lapply(srcData@data$HoverText, function(x){HTML(x)}),
              labelOptions = labelOptions(
                direction='top',
                offset=c(0,-10),
                style=list(
                  'background'='rgba(255,255,255,0.95)',
                  'border-color' = 'rgba(0,0,0,1)',
                  'border-radius' = '4px',
                  'border-style' = 'solid',
                  'border-width' = '1px'))
            ) %>%
            addLegend(
              pal = pal,
              title = 'Sentiment',
              values = ~ Sentiment,
              opacity = 0.75
            )
        }
      }
      
  })
    
  
  
    # Create the [map: Activity mutlti city]
    output$mapMultiCityTweeters <- renderLeaflet({
      leaflet(options = leafletOptions()) %>%
        addTiles(urlTemplate = "//{s}.tiles.mapbox.com/v3/jcheng.map-5ebohr46/{z}/{x}/{y}.png",
                 attribution = 'Maps by <a href="http://www.mapbox.com/">Mapbox</a>') %>%
        fitBounds(114, -11, 154, -44)
      
    })
    
    mapMultiCityTweetersZoomState <- reactive({
      zoom <- 4
      bounds <- list(
        west = 114,
        east = 154,
        north = 11,
        south = -44
      )
      
      if (!is.null(input$mapMultiCityTweeters_bounds)) {
        bounds <- input$mapMultiCityTweeters_bounds
        zoom <- input$mapMultiCityTweeters_zoom
      }
      
      list("zoom" = zoom, "bounds" = bounds)
    })
    
    
    observe({
      zs <- mapMultiCityTweetersZoomState()
      
      if (exists("zs") & !is.null(zs)) {
        precision <- if_else(zs$zoom <= 6, 1, zs$zoom / 4)
        
        geoActive <- geoTo %>%
          filter(
            zs$bounds$west <= X2 & X2 <= zs$bounds$east,
            zs$bounds$north >= Y2 & Y2 >= zs$bounds$south
          ) %>%
          mutate(X2 = round(X2*2, precision)/2,
                 Y2 = round(Y2*2, precision)/2) %>%
          group_by(X2, Y2) %>%
          summarise(Tally = n()) %>%
          mutate(Size = ifelse(Tally<10,3,log10(Tally)*4))
        
        
        leafletProxy("mapMultiCityTweeters",  data = geoActive) %>%
          clearMarkers() %>%
          addCircleMarkers(
            lng =  ~ X2,
            lat =  ~ Y2,
            radius = ~ Size,           
            stroke = FALSE,
            fillOpacity = 0.75,
            color = 'red'
          )
        
      }
    })
    
    #              -------------------------------------
    
    # Create the [map: Sentiment Analysis]
    output$mapAllSentByArea <- renderLeaflet({
      aid <- 4
      pal <- colorNumeric(c( "navy","yellow","red" ),NULL)
      
      leaflet(aas[[aid]]) %>%
        addTiles(urlTemplate = "//{s}.tiles.mapbox.com/v3/jcheng.map-5ebohr46/{z}/{x}/{y}.png",
                 attribution = 'Maps by <a href="http://www.mapbox.com/">Mapbox</a>') %>%
        fitBounds(114,-11, 154,-44)   %>%
        addPolygons(
          stroke = TRUE,
          smoothFactor = 1,
          fillOpacity = 0.5,
          weight = 1,
          opacity = 0.6,
          color = 'black',
          fillColor = ~ pal(Sentiment),
          highlightOptions = highlightOptions(
            color = 'red',
            weight = 3,
            bringToFront = TRUE
          )
        ) %>%
        addLegend(
          pal = pal,
          title = 'Sentiment',
          values = ~ Sentiment,
          opacity = 0.75
        )
    })
    
    mapAllSentByAreaZoomState <- reactive({
      zoom <- 4
      bounds <- list(
        west = 114,
        east = 154,
        north = 11,
        south = -44
      )
      
      if (!is.null(input$mapAllSentByArea_bounds)) {
        bounds <- input$mapAllSentByArea_bounds
        zoom <- input$mapAllSentByArea_zoom
      }
      
      list("zoom" = zoom, "bounds" = bounds)
    })
    
    observe({
      zs <- mapAllSentByAreaZoomState()
      
      if (exists("zs") & !is.null(zs)) {
        aid <- as.integer(input$AllSentByArea.areaId)
        sd <- aas[[aid]]
        
        if (!isTRUE(session$userData$mapAllSentByArea) &
            zs$zoom > 12) {
          session$userData$mapAllSentByArea <- TRUE
          updateRadioButtons(
            session,
            "AllSentByArea.areaId",
            label = "Area:",
            choices = c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2,
              "SA1" = 1
            ),
            selected = aid
          )
        }
        
        if (isTRUE(session$userData$mapAllSentByArea) &
            zs$zoom <= 12) {
          session$userData$mapAllSentByArea <- FALSE
          updateRadioButtons(
            session,
            "AllSentByArea.areaId",
            label = "Area:",
            choices = c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2
            ),
            selected = 2
          )
        }
        
        bb <- zs$bounds
        ff <-        ((sd$bbox1 <= bb$west &
                         bb$west <= sd$bbox3) | (sd$bbox1 <= bb$east &
                                                   bb$east <= sd$bbox3) |
                        (sd$bbox1 >= bb$west &
                           bb$east >= sd$bbox1) |
                        (sd$bbox3 >= bb$west &
                           bb$east >= sd$bbox3)
        ) &
          ((sd$bbox2 <= bb$south &
              bb$south <= sd$bbox4) |
             (sd$bbox2 <= bb$north & bb$north <= sd$bbox4) |
             (sd$bbox2 >= bb$south &
                bb$north >= sd$bbox2) |
             (sd$bbox4 >= bb$south & bb$north >= sd$bbox4)
          )
        
        srcData <- sd[ff,]
        
        if (!is.null(srcData)) {
          pal <- colorNumeric(c( "navy","yellow","red" ), NULL)
          
          leafletProxy("mapAllSentByArea", data = srcData) %>%
            clearShapes() %>%
            clearControls() %>%
            addPolygons(
              stroke = TRUE,
              smoothFactor = 1,
              fillOpacity = 0.5,
              weight = 1,
              opacity = 0.6,
              color = 'black',
              fillColor = ~ pal(Sentiment),
              highlightOptions = highlightOptions(
                color = 'red',
                weight = 3,
                bringToFront = TRUE
              ),
              label = lapply(srcData@data$HoverText, function(x){HTML(x)}),
              labelOptions = labelOptions(
                direction='top',
                offset=c(0,-20),
                style=list(
                  'background'='rgba(255,255,255,0.95)',
                  'border-color' = 'rgba(0,0,0,1)',
                  'border-radius' = '4px',
                  'border-style' = 'solid',
                  'border-width' = '1px'))
            ) %>%
            addLegend(
              pal = pal,
              title = 'Sentiment',
              values = ~ Sentiment,
              opacity = 0.75
            )
        }
        }
      })
      
      # ------------------------------------------------------------
      # new Activity
      
    output$mapRecentCityTweeters <- renderLeaflet({
      leaflet(data = recentLocations, options = leafletOptions()) %>%
        addTiles(urlTemplate = "//{s}.tiles.mapbox.com/v3/jcheng.map-5ebohr46/{z}/{x}/{y}.png",
                 attribution = 'Maps by <a href="http://www.mapbox.com/">Mapbox</a>') %>%
        addCircleMarkers(
          lng =  ~ Xloc,
          lat =  ~ Yloc,
          radius = ~ Size,
          stroke = FALSE,
          fillOpacity = 0.7,
          color = 'deeppink'
        ) %>%
        fitBounds(114,-11, 154,-44)
    })
    
   
         
      
      
      #              -------------------------------------
    # last 
    
    output$mapRecentSentBySA <- renderLeaflet({
      aid <- 4
      bid <- 2
      pal <- colorNumeric(c("navy", "azure", "red"), NULL)
      
      sd <- ras[[aid]]
      if (bid == 1) {
        sd@data %<>% mutate(Sentiment = coalesce(Relative, 0))
      }
      
      leaflet(sd) %>%
        addTiles(urlTemplate = "//{s}.tiles.mapbox.com/v3/jcheng.map-5ebohr46/{z}/{x}/{y}.png",
                 attribution = 'Maps by <a href="http://www.mapbox.com/">Mapbox</a>') %>%
        fitBounds(114, -11, 154, -44) %>%
        addPolygons(
          stroke = TRUE,
          smoothFactor = 1,
          fillOpacity = 0.5,
          weight = 1,
          opacity = 0.6,
          color = 'black',
          fillColor = ~pal(Sentiment),
          highlightOptions = highlightOptions(
            color = 'red',
            weight = 3,
            bringToFront = TRUE
          )
        ) %>%
        addLegend(
          pal = pal,
          title = 'Sentiment',
          values = ~Sentiment,
          opacity = 0.75
        )
    })
    
    mapRecentSentBySAZoomState <- reactive({
      zoom <- 4
      bounds <- list(
        west = 114,
        east = 154,
        north = 11,
        south = -44
      )
      
      if (!is.null(input$mapRecentSentBySA_bounds)) {
        bounds <- input$mapRecentSentBySA_bounds
        zoom <- input$mapRecentSentBySA_zoom
      }
      
      list("zoom" = zoom, "bounds" = bounds)
    })
    
    observe({
      zs <- mapRecentSentBySAZoomState()
      
      if (exists("zs") & !is.null(zs)) {
        aid <- as.integer(input$RecentSentBySA.areaId)
        bid <- as.integer(input$RecentSentBySA.baseId)
        
        sd <- ras[[aid]]
        if (bid == 1) {
          sd@data %<>% mutate(Sentiment = coalesce(Relative, 0))
        }
        
        if (!isTRUE(session$userData$mapRecentSentBySA) &
            zs$zoom > 12) {
          session$userData$mapRecentSentBySA <- TRUE
          updateRadioButtons(
            session,
            "RecentSentBySA.areaId",
            label = "Area:",
            choices = c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2,
              "SA1" = 1
            ),
            selected = aid
          )
        }
        
        if (isTRUE(session$userData$mapRecentSentBySA) &
            zs$zoom <= 12) {
          session$userData$mapRecentSentBySA <- FALSE
          updateRadioButtons(
            session,
            "RecentSentBySA.areaId",
            label = "Area:",
            choices = c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2
            ),
            selected = 2
          )
        }
        
        bb <- zs$bounds
        ff <- ((sd$bbox1 <= bb$west &
                  bb$west <= sd$bbox3) | (sd$bbox1 <= bb$east &
                                            bb$east <= sd$bbox3) |
                 (sd$bbox1 >= bb$west &
                    bb$east >= sd$bbox1) |
                 (sd$bbox3 >= bb$west &
                    bb$east >= sd$bbox3)
        ) &
          ((sd$bbox2 <= bb$south &
              bb$south <= sd$bbox4) |
             (sd$bbox2 <= bb$north & bb$north <= sd$bbox4) |
             (sd$bbox2 >= bb$south &
                bb$north >= sd$bbox2) |
             (sd$bbox4 >= bb$south & bb$north >= sd$bbox4)
          )
        
        srcData <- sd[ff,]
        
        if (!is.null(srcData)) {
          pal <- colorNumeric(c("navy", "azure", "red"), NULL)
          
          leafletProxy("mapRecentSentBySA", data = srcData) %>%
            clearShapes() %>%
            clearControls() %>%
            addPolygons(
              stroke = TRUE,
              smoothFactor = 1,
              fillOpacity = 0.5,
              weight = 1,
              opacity = 0.6,
              color = 'black',
              fillColor = ~pal(Sentiment),
              highlightOptions = highlightOptions(
                color = 'red',
                weight = 3,
                bringToFront = TRUE
              ),
              label = lapply(srcData@data$HoverText, function(x) { HTML(x) }),
              labelOptions = labelOptions(
                direction = 'top',
                offset = c(0, -10),
                style = list(
                  'background' = 'rgba(255,255,255,0.95)',
                  'border-color' = 'rgba(0,0,0,1)',
                  'border-radius' = '4px',
                  'border-style' = 'solid',
                  'border-width' = '1px'))
            ) %>%
            addLegend(
              pal = pal,
              title = 'Sentiment',
              values = ~Sentiment,
              opacity = 0.75
            )
        }
      }
      
    })
    
  
  
  
  
  
  
}
