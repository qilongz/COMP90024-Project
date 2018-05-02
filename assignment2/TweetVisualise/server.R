library(leaflet)
library(RColorBrewer)
library(scales)

library(lattice)
library(tidyverse)



function(input, output, session) {
  ## Interactive Map -------------------------------------------------
  
  # Create the [map: Activity Distribution]
  output$mapExploreDistribution <- renderLeaflet({
    leaflet(options = leafletOptions()) %>%
      addTiles(urlTemplate = "//{s}.tiles.mapbox.com/v3/jcheng.map-5ebohr46/{z}/{x}/{y}.png",
               attribution = 'Maps by <a href="http://www.mapbox.com/">Mapbox</a>') %>%
      fitBounds(114,-11, 154,-44)
    
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
  
  
  # Create the [map: Activity Distribution]
  output$mapExploreVolume <- renderLeaflet({
    leaflet(options = leafletOptions()) %>%
      addTiles(urlTemplate = "//{s}.tiles.mapbox.com/v3/jcheng.map-5ebohr46/{z}/{x}/{y}.png",
               attribution = 'Maps by <a href="http://www.mapbox.com/">Mapbox</a>') %>%
      fitBounds(114,-11, 154,-44)
    
  })
  
  mapExploreVolumeZoomState <- reactive({
    zoom <- 4
    bounds <- list(
      west = 114,
      east = 154,
      north = 11,
      south = -44
    )
    
    if (!is.null(input$mapExploreVolume_bounds)) {
      bounds <- input$mapExploreVolume_bounds
      zoom <- input$mapExploreVolume_zoom
    }
    
    list("zoom" = zoom, "bounds" = bounds)
  })
  
  
  observe({
    zs <- mapExploreVolumeZoomState()
    
    if (exists("zs") & !is.null(zs)) {
      precision <- if_else(zs$zoom <= 6, 1, zs$zoom / 4)
      
      
      activeLocations <- allLocations %>%
        filter(
          zs$bounds$west <= Xloc & Xloc <= zs$bounds$east,
          zs$bounds$north >= Yloc &
            Yloc >= zs$bounds$south
        )
      
      
      leafletProxy("mapExploreVolume", data = activeLocations) %>%
        clearMarkers() %>%
        addCircleMarkers(
          lng =  ~ Xloc,
          lat =  ~ Yloc,
          radius = 4,
          stroke = FALSE,
          fillOpacity = 0.75,
          color = 'red',
          clusterOptions = markerClusterOptions(chunkedLoading =
                                                  TRUE)
        )
      
    }
  })
  
  
  
  
  
  
  
  
  ## Sentiment Maps -------------------------------------------------
  
  # Create the [map: Sentiment Analysis]
  output$mapAgeByArea <- renderLeaflet({
    aid <- 4
    pal <- colorNumeric("viridis", NULL)
    
    leaflet(sas[[aid]]) %>%
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
        fillColor = ~ pal(median_age_persons),
        highlightOptions = highlightOptions(
          color = 'red',
          weight = 3,
          bringToFront = TRUE
        ),
        label = ~ Name
      ) %>%
      addLegend(
        pal = pal,
        title = 'Median Age',
        values = ~ median_age_persons,
        opacity = 0.75
      )
  })
  
  
  
  observe({
    aid <- as.integer(input$AgeBySA.areaId)
    srcData <- sas[[aid]]
    
    if (!is.null(srcData)) {
      pal <- colorNumeric("viridis", NULL)
      
      leafletProxy("mapAgeByArea", data = srcData) %>%
        clearShapes() %>%
        clearControls() %>%
        addPolygons(
          stroke = TRUE,
          smoothFactor = 1,
          fillOpacity = 0.5,
          weight = 1,
          opacity = 0.6,
          color = 'black',
          fillColor = ~ pal(median_age_persons),
          highlightOptions = highlightOptions(
            color = 'red',
            weight = 3,
            bringToFront = TRUE
          ),
          label = ~ Name
        ) %>%
        addLegend(
          pal = pal,
          title = 'Median Age',
          values = ~ median_age_persons,
          opacity = 0.75
        )
    }
    
  })
  
  #              -------------------------------------
  
  # Create the [map: Sentiment Analysis]
  output$mapSentimentByArea <- renderLeaflet({
    aid <- 4
    pal <- colorNumeric("viridis", NULL)
    
    leaflet(sas[[aid]]) %>%
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
        ),
        label = ~ Name
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
      sd <- sas[[aid]]
      
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
        pal <- colorNumeric("viridis", NULL)
        
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
            label = ~ Name
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
  
}
