# EasyHistory

This is a custom control that enable users to see atributes audit history using a simple interface 😊.

![alt text](https://github.com/VinnyDyn/EasyHistory/blob/master/0.gif)

It is compatible with these attribute types:

- Whole.None
- TwoOptions
- DateAndTime.DateOnly
- DateAndTime.DateAndTime
- Decimal
- FP
- Multiple
- Currency
- OptionSet
- SingleLine.Email
- SingleLine.Text
- SingleLine.TextArea
- SingleLine.URL
- SingleLine.Ticker
- SingleLine.Phone

### \node_modules\@types\xrm\index.d.ts\Xrm.ExecuteResponse
Change the attribute body from 'string' to 'ReadableStream':
```typescript
 interface ExecuteResponse {
        /**
         * (Optional). Object.Response body.
         */
        body: ReadableStream;
```
