# key-vault 7.0
Possible to create, delete and get keys. Each key belongs to a account, and accounts can be configured in startup or by calling accounts endpoint.

```
APIEnvironment__Accounts__<idx_started_by_zero>__Account: <name>
APIEnvironment__Accounts__<idx_started_by_zero>__Token: <token>
```

Endpoints:

```
/accounts (post, get, delete)
/secrets (put, get, delete)
```

Accounts POST Payload:
```
/accounts
{
    "Name": "<name>
}
```

Secrets PUT Payload:
```
/secrets (put, get, delete)
{
    "value": "<value>"
}
```