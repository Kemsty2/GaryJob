
# Sample Workflow with Elsa

A simple workflow build with Elsa



## Tech Stack

**Client:** Spencil.Js

**Server:** .Net Core 5.0, Elsa


## Deployment

To deploy this project run

```shell
  $ git clone https://github.com/Kemsty2/GaryJob.git
  $ cd GaryJob
  $ dotnet restore
  $ dotnet run --project /src/GaryJob.Api/GaryJob.Api.csproj
```




## Usage/Examples

To run the workflow, simply run this command with curl or postman
```shell
curl --location --request POST 'https://localhost:5001/files' \
--header 'Content-Type: application/json' \
--data-raw '{
    "test": "test"
}'
```

