## Data processing
Scripts in this folder require a valid Spark environment to run, which association of Hadoop HDFS and Hive, a valid Java environment and Python 3.

To run the scripts, the following python packages is required:  
-pyspark  
-pandas  
-pywebhdfs  
-nltk  
 
Make sure the environment path is set :
  
$ export SPARK\_HOME=/usr/hdp/current/spark2-client/  
$ export PYSPARK\_PYTHON=python3

# uni\_data\_collector.py
Function: Extract data from the university twitter data 
 
Usage:  
Local: $ spark-submit /path/to/this .py/file  </path/to/target/dir>  </path/to/output/dir>  <ouput_table-name>  


Cluster: $ spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file  </path/to/target/dir>  </path/to/output/dir>  <ouput_table-name>

# twitter\_data\_collector.py
Function: Extract data from the twitter harvest 
data 
 
Usage:  
Local: $ spark-submit /path/to/this .py/file  </path/to/target/dir>  </path/to/output/dir>  <ouput_table-name>  


Cluster: $ spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file  </path/to/target/dir>  </path/to/output/dir>  <ouput_table-name>

# data\_analysis.py   
Function: This script is used to analyse extracted data and generate csv file for analysis result, which including tweet count and sentiment analysis score.  

Usage:  
Local: $ spark-submit /path/to/this .py/file <table_name>  </path/to/local/dir> 
 
Cluster: $ spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file <table_name>  </path/to/local/dir>  



# json\_gen.py
Function: This script is used to generate a JSON file, and then one copy will be stored in hdfs and another copy will be write into local directory  

Usage:  
Local: $ spark-submit /path/to/this .py/file <table_name> </path/to/hdfs/file>  </path/to/local/file>  

Cluster: $ spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file  <table_name> </path/to/hdfs/file>  </path/to/local/file> 

# drop\_table.py  
Function: This script is used to delete specific hive table   

Usage:
Local: $ spark-submit /path/to/this .py/file   <ouput_table-name>

Cluster: $ spark-submit --master yarn --deploy-mode cluster /path/to/this .py/file  <ouput_table-name>