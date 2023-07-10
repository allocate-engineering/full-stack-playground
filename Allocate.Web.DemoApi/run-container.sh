#!/bin/bash

#####################################################
## Entry run script for running in AWS fargate
##
## Setup environment variables and run server api
#####################################################

STAGE=$DOTNET_ENVIRONMENT  # dev, test, prod

## if no stage exists, we are running in local mode
if [ -z "$STAGE" ]; then
  export STAGE="Local"
  export DOTNET_ENVIRONMENT="Local"
fi

## the environment used to query secret manager
## lowercase version of all envs
SM_ENV=$(echo $STAGE | awk '{print tolower($0)}')

## Set the appropriate dotnet environment and other misc
## environment variables.  Doing that here, rather than
## during terraform setup so that we can have control
## over it at runtime rather then re-running terraform scripts

## Per-environment
if [ "$STAGE" = "Local" ]; then
  echo "Local environment"
elif [ "$STAGE" = "Development" ]; then
  echo "Development environment"
  export SM_ENV="dev"
elif [ "$STAGE" = "Demo" ]; then
  echo "Demo environment"
  export SM_ENV="demo"
elif [ "$STAGE" = "Staging" ]; then
  echo "Staging environment"
  export SM_ENV="staging"
elif [ "$STAGE" = "Production" ]; then
  echo "Production environment"
fi

echo "** running Allocate-Demo-Api for environment $DOTNET_ENVIRONMENT"


## Only print out these environment variables if you have to
## for debugging purposes, as we do not want to put them in
## the logs for security purposes (they contain passwords)
echo "##################################################"
echo "######### Runtime Environment Variables ##########"
echo "##################################################"

echo "DOTNET_ENVIRONMENT: $DOTNET_ENVIRONMENT"

dotnet "Allocate.Web.DemoApi.dll"

