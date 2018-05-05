
# Activity related configuration

# Module UI function

activityInput <- function(id) {
  # Create a namespace function using the provided id
  ns <- NS(id)
  
  tagList(
    
    tabPanel(
      "Introduction",
      id = ns("introduction"),
      h4("Introduction to data, sourcing, etc.")
    ),
    
    tabPanel(
      "Activity",
             id = ns("activity"),
             div(class = "outer",
                 tags$head(
                   includeCSS("styles.css")
                 ))
      ),
    
    tabPanel(
      "Activity Distribution",
      id = ns("distribution"),
      
      div(
        class = "outer",
        tags$head(includeCSS("styles.css")),
        leafletOutput(ns("mapDistribution"), width = "100%", height = "100%")
      )
    ),
    
    tabPanel(
      "Activity Volume",
      id = ns("volume"),
      
      div(
        class = "outer",
        tags$head(includeCSS("styles.css")),
        leafletOutput(ns("mapVolume"), width = "100%", height = "100%")
      )
    )
  )
}

# -------------------------------------------------------------------------------------------------------

# Module server function

activityServer <- function(input, output, session) {
  
  # The selected file, if any
  userFile <- reactive({
    # If no file is selected, don't do anything
    validate(need(input$file, message = FALSE))
    input$file
  })
