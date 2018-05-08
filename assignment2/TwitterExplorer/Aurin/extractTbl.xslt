<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:wfs="http://www.opengis.net/wfs"
                xmlns:gml="http://www.opengis.net/gml"
                xmlns:aurin="http://www.aurin.org.au"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">

  <!-- <xsl:output method="xml" indent="yes" /> -->

  <xsl:output method="html" indent="yes" />

  <xsl:template match="/">

    <html>
      <head>
        <style type="text/css">
          <xsl:comment>
            body		{
            font-family: "Trebuchet MS";
            }

            table		{
            border:.5pt solid #888;

            font-family: "Trebuchet MS";
            font-size: x-small;
            border-collapse:collapse;
            }

            .none		{
            border:1pt solid white;
            vertical-align:top;
            }

            th		{
            border:.5pt solid #ccc;

            text-align:left;
            padding-top:0px;
            padding-bottom:0px;
            vertical-align:top;
            padding-left:3px;
            padding-right:3px;

            color:white;
            background-color:#888;
            white-space:nowrap;
            }

            th.mid		{
            border-top:.5pt solid #888;
            border-bottom:.5pt solid #888;

            text-align:center;
            color:black;
            background-color:#fff;
            }

            td		{
            border:.5pt solid #ccc;
            vertical-align:top;
            word-wrap:normal;
            text-align:right;
            padding-left:3px;
            padding-right:3px;
            padding-top:0px;
            padding-bottom:0px;
            white-space:nowrap;
            }

            .dbl		{
            border:3pt double #ccc;
            }

            .sep		{
            border-left:3pt double #ccc;
            }

            .under		{
            border-bottom:3pt double #ccc;
            }


            .rght		{
            text-align:right;
            }

            .lft		{
            text-align:left;
            border:1pt solid white;
            }

            .space		{
            text-align:left;
            border:1pt solid white;
            padding-left:50px;
            }

            .empty		{
            padding-left:50px;
            }


            .high	{
            background-color:lightblue;
            }

            .top		{
            vertical-align:top;
            }


            .c		{
            font-style:italic;
            }

            .flag	{
            background-color:yellow;
            }
            .no		{
            white-space:nowrap;
            }
            .bold	{
            font-weight:bold;
            font-size:110%;
            }
            ul {
            margin-top:1px;
            margin-bottom:1px;
            mso-data-placement:same-cell;
            }
            li {
            list-style-type:none;
            list-style-position:outside;
            }
            .disc {
            list-style-type:circle;
            }
            .bull {
            list-style-type:disc;
            }
            br {
            mso-data-placement:same-cell;
            }
            .circle {
            margin-left:18px;
            list-style-type:circle
            }
            .dash {
            margin-left:18px;
            }
            br {
            mso-data-placement:same-cell;
            }
          </xsl:comment>
        </style>
      </head>

      <body>
        <table cellpadding="3" cellspacing="0" border="1">
          <xsl:apply-templates select='//gml:featureMember' />
        </table>
      </body>
    </html>
  </xsl:template>


  <xsl:template match="gml:featureMember">
    <tr>
      <td>
        <xsl:value-of select='.//aurin:sa4_name16' />
      </td>
      <td>
        <xsl:value-of select='.//aurin:median_tot_prsnl_inc_weekly' />
      </td>

    </tr>
  </xsl:template>




  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>

