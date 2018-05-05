library(leaflet)


navbarPage(
  "Menu",
  id = "nav",
  windowTitle = "Team 40 - COMP90024_2018_SM1: Cluster and Cloud Computing - 2018",
  
  navbarMenu(
    "Data Exploration",
    
    activityInput("Activity"),
    
    
    tabPanel("Introduction",
             id = "explore.introduction",
             h4("Introduction to data, sourcing, etc.")),
    
    tabPanel(
      "Activity Distribution",
      id = "explore.activity",
      div(
        class = "outer",
        tags$head(# Include our custom CSS
          includeCSS("styles.css"),
          includeScript("gomap.js")),
        # render map
        leafletOutput(
          "mapExploreDistribution",
          width = "100%",
          height = "100%"
        )
      )
    ),
    
    tabPanel(
      "Activity Volume",
      id = "explore.volume",
      div(
        class = "outer",
        
        tags$head(# Include our custom CSS
          includeCSS("styles.css"),
          includeScript("gomap.js")),
        
        # render map
        leafletOutput("mapExploreVolume", width = "100%", height = "100%")
      )
    )
  ),
  
  
  
  navbarMenu(
    "Sentiment Analysis",
    
    tabPanel(
      "Age By Statistical Area",
      id = "sentiment.AgeBySA ",
      
      div(
        class = "sentiment",
        
        tags$head(# Include our custom CSS
          includeCSS("styles.css"),
          includeScript("gomap.js")),
        
        
        div(class = "wait", "Please wait while data loads ...."),
        
        # render map
        leafletOutput("mapAgeByArea", width = "100%", height = "100%"),
        
        absolutePanel(
          id = "controls.AgeBySA",
          class = "panel panel-default max",
          fixed = TRUE,
          draggable = TRUE,
          top = 180,
          left = 10,
          right = "auto",
          bottom = "auto",
          width = 70,
          height = "auto",
          
          radioButtons(
            "AgeBySA.areaId",
            "Area:",
            c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2
            ),
            selected = 4,
            inline = FALSE
          )
          
        )
      )
    ),
    
    
    tabPanel(
      "Sentiment By Statistical Area",
      id = "sentiment.SentimentBySA",
      
      div(
        class = "sentiment",
        
        tags$head(# Include our custom CSS
          includeCSS("styles.css"),
          includeScript("gomap.js")),
        
        
        div(class = "wait", "Please wait while data loads ...."),
        
        # render map
        leafletOutput("mapSentimentByArea", width = "100%", height = "100%"),
        
        absolutePanel(
          id = "controls.SentimentBySA",
          class = "panel panel-default max",
          fixed = TRUE,
          draggable = TRUE,
          top = 180,
          left = 10,
          right = "auto",
          bottom = "auto",
          width = 70,
          height = "auto",
          
          radioButtons(
            "SentimentBySA.areaId",
           label= "Area:",
           choices= c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2
            ),
            selected = 4,
            inline = FALSE
          )
        )
      )
    )
  ),
  
  tabPanel("User Analysis", id = "user ",
           div())  ,
  
  tags$head(
    div(
      style = "float:left;padding-left:15px",
      em("COMP90024_2018_SM1: Cluster and Cloud Computing - 2018")
    ),
    div(style = "float:right;margin-right:20px;", strong("Team 40")),
    h4(style = "text-align:center;", "Interactive Tweet Explorer")
  )
  
)
