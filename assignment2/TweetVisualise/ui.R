library(leaflet)
library(ggmap)

navbarPage(
  "Menu",
  id = "nav",
  windowTitle = "Team 40 - COMP90024_2018_SM1: Cluster and Cloud Computing - 2018",
  
  navbarMenu(
    "Activity",
    
    tabPanel(
      "Introduction",
      id = "explore.introduction",
      fluidRow(mainPanel(
        h2(
          "Australian Social Media Analytics - Interstate Domestic Traveller Investigator"
        ),
        
        p(
          "An application has been developed to examine the behaviour of Australian interstate travellers.
          The application combines current and historical Twitter data with 2016 Australian Census data to provide insight in
          to the behaviour of individuals who travel between state capitals within Australia."
        ),
        br(),
        
        
        h4("Summary of Calibration Data"),
        
        p(
          "A set of historical Twitter data was used to calibrate various aspects of the tool.
          Historical tweets, already  captured by the University, were used to calibrate a sentiment analysis capability."
        ),
        
        
        p("The calibration set consisted of: "),
        
        tags$ul(
          tags$li("87,028,224 geo-located tweets,"),
          tags$li("covering the folowing Australian capital cities:"),
          tags$ul(
            tags$li("Adelaide"),
            tags$li("Brisbane"),
            tags$li("Canberra"),
            tags$li("Hobart"),
            tags$li("Melbourne"),
            tags$li("Perth"),
            tags$li("Sydney")
          ),
          tags$li("covering the period from 1st July, 2017 to 31st March, 2018"),
          tags$li(
            "619,056 (0.624%) of these tweets consisted the GPS coordinates (Exact Location Tweets)"
          ),
          tags$li("covering 102,072 distinct geographic locations")
        ),
        
        br(),
        h4("Explorative Statistics")
        
        )),
      
      fluidRow(plotOutput(outputId = "plotActivityAllVolByMth"))    ,
      br(),
      fluidRow(plotOutput(outputId = "plotActivityNewVolByMth"))
      ),
    
    
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
        ),
        tags$div(id = "cite",
                 "Volume of tweets by location")
      )
    )
    ),
  
  
  
  navbarMenu(
    "Sentiment",
    
    tabPanel(
      "Approach",
      id = "sentiment.introduction",
      
      h2("Sentiment Analysis "),
      
      
      p(
        "An existing sentiment analyser formulated for Social Media research was used to establish a baseline sentiment for 
        all the available tweet data.
        The specific analyser (VADER - ", tags$em("Valence Aware Dictionary and sEntiment Reasoner"), ") is a lexicon and 
        rule-based sentiment analysis tool.
        The tool was choosen as is specifically attuned to sentiments expressed in social media."
      ),
      
      p(
        "Configuration and code from multiple sources (https://github.com/cjhutto/vaderSentiment  
          and  https://github.com/codingupastorm/vadersharp)
        were merged.  A number of performance issue where addressed with the available implementations."
      ),
      
      p(
        "The analyser is capability generating  scores from multiple perspectives.
        The ", tags$b("compound"), "score  provides a single unidimensional measure of sentiment for a given sentence.
        The compound score is computed by summing the valence scores of each word in the lexicon,
        adjusted according to the rules, and then normalized to be between -1 (most extreme negative)
        and +1 (most extreme positive)."
      ),
      
      p(
        "It can be used standardized thresholds for classifying sentences as either positive, neutral, or negative.
        Typical threshold values (used with approach) are:"
      ),
      
      tags$ul(
        tags$li("positive sentiment: compound score >= 0.05"),
        tags$li(
          "neutral sentiment: (compound score > -0.05) and (compound score < 0.05)"
        ),
        tags$li("negative sentiment: compound score <= -0.05")
      ),
      
      
      p("Specifically, the scoring  process:"),
      
      tags$ul(
        tags$li(
          "scores word for positive & negative associations (e.g. awesome +)  (e.g. dickhead -)"
        ),
        tags$li(
          "increased  sentiment with capitalised words (e.g. I LOVE my dog,  I HATE you)"
        ),
        tags$li("increased positive sentiment for !!!!!"),
        tags$li("understands some double negatives (e.g. not bad)"),
        tags$li(
          "understand old smiles  :)   ):<     - it has prescribed weighting for each of these combinations."
        ),
        tags$li(
          "understands  emojis  -  emojis are converted to word equivalents before sentiment analysis is applied"
        )
      ),
      br(),
      br(),
      
      h4(
        "A VADER sentiment analysis was applied to  87,028,224  distinct Geo-located tweets."
      )
      
      ),
    
    tabPanel(
      "By Location",
      
      h2("By Location"),
      fluidRow(plotOutput(outputId = "plotAverageSentimentByMthByLocation"))    ,
      br(),
      br(),
      h5(
        "Sentiment analysis is heavily distorted by large volume of neutral tweets."
      ),
      fluidRow(plotOutput(outputId = "plotDistributionSentimentAll")),
      br(),
      h5(
        "Scaling by total number of tweets per location and excluding central neutral tweets, it's clear there is a skew in the distribution.."
      ),
      fluidRow(
        plotOutput(outputId = "plotDistributionSentimentAllNormalised")
      )
    ),
    
    tabPanel(
      "By Time of Day",
      
      h2("By Time of Day"),
      fluidRow(plotOutput(outputId = "plotAverageSentimentByTimeOfDay"))    ,
      br(),
      br(),
      h5(
        "Scaling by total number of tweets per day and excluding central neutral tweets."
      ),
      fluidRow(
        plotOutput(outputId = "plotAverageSentimentByFilteredTimeOfDay")
      )
    ),
    tabPanel(
      "By Day of Week",
      
      h2("By Day of Week"),
      fluidRow(plotOutput(outputId = "plotAverageSentimentByDayofWeek"))    ,
      br(),
      br(),
      h5(
        "Scaling by total number of tweets per week and excluding central neutral tweets."
      ),
      
      fluidRow(
        plotOutput(outputId = "plotAverageSentimentByFilteredDayOfWeek")
      )
      
      
    ),
    tabPanel(
      "Basedlined Overview",
      
      h2("Basedlined Results"),
      
      p("The overall means for the sentiment scores were:"),
      tags$ul(
        tags$li(tags$b("12.28%"), " - for all  Geo-tagged tweets"),
        tags$li(
          tags$b("21.34%"), " - for all  Geo-tagged tweets, with the central neutral tweets (-5% to 5%) excluded"
        ),
        tags$li(
          tags$b("39.52%"), " - exact-position (GPS tagged) tweets - netural core (-0.05 to 0.05) excluded"
        ), 
        tags$li(
          tags$b("44.09%"), " - not in home city tweeters  - netural core (-0.05 to 0.05) excluded"
        )
      ),
      br(),
     
      
      p(
        "It's assumed that these percentages represent the baseline calibration for our sentiment analyser
        (as configured during calibration).",br(),"
        The sample statistics were baselined to allow relative differences to be observed."
      ),
      br(),
      br(),
      
      fluidRow(plotOutput(outputId = "plotBaseByLocation"))    ,
      br(),
      fluidRow(plotOutput(outputId = "plotBaseByHour")),
      br(),
      fluidRow(plotOutput(outputId = "plotBaseByDay"))
      ),
    
    
    
    tabPanel(
      "Facets",
      
      h2("Location & Day of Week Facet"),
      fluidRow(
        plotOutput(
          outputId = "plotDayLocationFacet",
          height = 600,
          width = 700
        )
      )  ,
      
      br(),
      h2("Location vs Day of Week - Time of Day  Facet"),
      fluidRow(plotOutput(outputId = "plotLocationFullFacet", height = 800)),
      
      br(),
      h2("Day of Week Vs Location - Time of Day  Facet"),
      fluidRow(plotOutput(outputId = "plotDayOfWeekFullFacet", height = 800))
      
    ),
    
    tabPanel(
      "Sentiment By Region",
      
      div(
        class = "outer",
        
        tags$head(# Include our custom CSS
          includeCSS("styles.css")),
        
        div(class = "wait", "Please wait while data loads ...."),
        
        # render map
        leafletOutput("mapAllSentByArea", width = "100%", height = "100%"),
        
        absolutePanel(
          id = "controls.AllSentByArea",
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
            "AllSentByArea.areaId",
            label = "Area:",
            choices = c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2
            ),
            selected = 4,
            inline = FALSE
          )
        ),
        
        tags$div(
          id = "cite",
          "Relative sentiment of Tweeters who post tweets with exact GPS coordinates"
        )
      )
    )
    ),
  
  
  navbarMenu(
    "Mobility",
    
    tabPanel(
      "Introduction",
      
      h2("Tweeter Mobility "),
      
      p(
        "It was observed that tweets were originated by a proportion of posters from multiple cities.",
        br(),
        "
        Assuming that the city where the tweeter posted the most tweets, was likely to be the  ",
        tags$b("home"),
        " city for a poster,", br(),"
        it was believed that some insight to the
        posters mobility and behaviour can be   obtained by analysing their  activity in their non-home cities. "
      ),
      
      h4("Assumptions"),
      
      p("The analysis is based on the following assumptions:"),
      
      tags$ul(
        tags$li(
          "the city with the most tweets for a given poster, is the home city of the poster"
        ),
        tags$li(
          "users that poster from multiple geographic locations are likely to be actual end-users not businesses (e.g. bars, venues, etc.).
          ", br(),
        "There are some small number of services (e.g. 'Where Pets Are Found', 'CEB Jobs' ) that post from multiple cities.
          ", br(),"These anomalies have not been removed in this analysis."
        ),
        tags$li(
          "that any tweets origniated by the poster, while not in their home city, are done while they are travelling."
        )
        ),
      
      br(),
      p("The following steps were undertaken: "),
      tags$ul(
        tags$li(
          "from the 87,028,224 geo-located tweets, we identified 44,587 individual tweet posters"
        ),
        tags$li(
          "we observed that 31,892 (72%) of the individual users had tweeted from more than one captial city"
        ),
        tags$li(
          "and for each of these multi-city tweeters, we identified which city they most frequently tweeted from.
          ", br(),"This was assumed to be there home city."
        )
        ),
      br(),
      p("Some additional analysis was under taken on the exact-position (GPS tagged) tweets:"),
      tags$ul(
        tags$li("from the 619,056 exact-position (GPS tagged) tweets"),
        tags$li("we identified the 246,845 tweets "),
        tags$li("posted by the 31,892 multi-city travellers"),
        tags$li("when not in their home city.")
      ),
      br(),
      p("Finally, for the 31,892 multi-city travellers:"),
      tags$ul(
        tags$li("we collected any exact-position tweets they posted in the last week"),
        tags$li("identified locations they posted from"),
        tags$li("and applied sentiment analysis to what they were tweeting.")
      )
      
      
      ), 
    tabPanel(
      "Movement of Mobile Tweeters",
      h2("Movement of Mobile Tweeters "),
      fluidRow(plotOutput(outputId = "plotNumFromMelb"))    ,
      br(),
      fluidRow(plotOutput(outputId = "plotNumToMelb"))
    ),
    tabPanel(
      "Activity of Mobile Tweeters",
      h2("Tweeting Activity of Mobile Tweeters "),
      fluidRow(plotOutput(outputId = "plotVolFromMelb"))    ,
      br(),
      fluidRow(plotOutput(outputId = "plotVolToMelb"))
    ),
    
    tabPanel(
      "Sample Destinations of Travellers",
      h3(
        "Selected Sample Locations that inter-city Travellers have tweeted from "
      ),
      br(),
      h4(
        'Locations tweeted from by non-Melbourne residents who tweeted when visiting inner-Melbourne'
      ),
      fluidRow(plotOutput(outputId = "mapInnerMelbDest", height = 800))    ,
      br(),
      h4(
        'Locations tweeted from by non-Sydney residents who tweeted when visiting inner-Sydney'
      ),
      fluidRow(plotOutput(outputId = "mapInnerSydDest", height = 800))
    ),
    
    tabPanel(
      "Traveller Tweet Locations",
      
      id = "multiLocations",
      div(
        class = "outer",
        
        tags$head(# Include our custom CSS
          includeCSS("styles.css")),
        # render map
        
        div(class = "wait", "Please wait while data loads ...."),
        
        # render map
        leafletOutput("mapMultiCityTweeters", width = "100%", height = "100%"),
        
        tags$div(id = "cite", "Tweet from locations of inter-city travellers")
      )
    ),
    
    tabPanel(
      "Sentiment By Region",
      
      div(
        class = "outer",
        
        tags$head(# Include our custom CSS
          includeCSS("styles.css")),
        
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
          width = 80,
          height = "auto",
          
          radioButtons(
            "SentimentBySA.areaId",
            label = "Area:",
            choices = c(
              "SA4" = 4,
              "SA3" = 3,
              "SA2" = 2
            ),
            selected = 4,
            inline = FALSE
          ),
          radioButtons(
            "SentimentBySA.baseId",
            label = "Baseline:",
            choices = c("Relative" = 2,
                        "Region" = 1),
            selected = 2,
            inline = FALSE
          )
        ),
        
        tags$div(
          id = "cite",
          "Relative sentiment of inter-city travellers when tweeting away from home"
        )
      )
      ),
      
      
      
     
      "----",
    "Recent Activity",
      
      tabPanel(
        "Traveller Tweet Locations",
        
        id = "recentMultiLocations",
        div(
          class = "outer",
          
          tags$head(# Include our custom CSS
            includeCSS("styles.css")),
          # render map
          
          div(class = "wait", "Please wait while data loads ...."),
          
          # render map
          leafletOutput("mapRecentCityTweeters", width = "100%", height = "100%"),
          
          tags$div(id = "cite", "Locations that inter-city travellers have from tweeted during the last week")
        )
      ),
      
      
      tabPanel(
        "Sentiment By Region",
        
        div(
          class = "outer",
          
          tags$head(# Include our custom CSS
            includeCSS("styles.css")),
          
          div(class = "wait", "Please wait while data loads ...."),
          
          # render map
          leafletOutput("mapRecentSentBySA", width = "100%", height = "100%"),
          
          absolutePanel(
            id = "controls.RecentSentBySA",
            class = "panel panel-default max",
            fixed = TRUE,
            draggable = TRUE,
            top = 180,
            left = 10,
            right = "auto",
            bottom = "auto",
            width = 80,
            height = "auto",
            
            radioButtons(
              "RecentSentBySA.areaId",
              label = "Area:",
              choices = c(
                "SA4" = 4,
                "SA3" = 3,
                "SA2" = 2
              ),
              selected = 4,
              inline = FALSE
            ),
            radioButtons(
              "RecentSentBySA.baseId",
              label = "Baseline:",
              choices = c("Relative" = 2,
                          "Region" = 1),
              selected = 2,
              inline = FALSE
            )
          ),
          
          tags$div(
            id = "cite",
            "Sentiment of inter-city travellers during the last week"
          )
        )
        
      ),
      
      
      
      tags$head(
        div(
          style = "float:left;padding-left:15px",
          em("COMP90024_2018_SM1: Cluster and Cloud Computing - 2018")
        ),
        div(style = "float:right;margin-right:20px;", strong("Team 40")),
        h4(style = "text-align:center;", "Interstate Domestic Traveller Investigator")
      )
      
    )
      )
  