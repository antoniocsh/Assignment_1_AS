1. Read Before You Touch — Architecture Analysis
Before writing a single line of instrumentation code, spend time understanding the codebase.
Your documentation must answer:
- How are the layers organised and what are the dependency rules between them?
- How does nopCommerce handle events internally — what is IEventPublisher and
how is it used?
- Where does the code make it easy to add observability, and where does it make it hard?
- What would you need to change structurally to instrument it properly — and is that
change worth making?
This section is not a summary of the README. It is your architectural reading of the system.
2. Instrument One User Flow End-to-End
Customer searches and views a product -> Catalogue · Search · Pricing
Your instrumentation must cover:
- Distributed tracing — spans from HTTP entry point through service calls to database
- At least two custom metrics that provide genuine operational insight (not just request count — that's already available)
- Sensitive data exclusion — no emails, payment details or PII in traces or logs
The metrics you choose must be justified. "I added this because it was easy" is not a
justification. "This metric would tell an operator that the checkout pipeline is degrading before
users start seeing errors" is.
3. Grafana Dashboard
Set up a Grafana dashboard that visualises your instrumented flow. It must include:
- Trace view for the selected flow (via Jaeger or Tempo)
- At least one panel per custom metric
- A panel showing error rate for the flow
The dashboard should tell a story — someone who has never seen your code should be able to
look at it and understand what is happening in the system.
4. Load Test
Provide a load test script (k6, Locust, or JMeter) that drives your selected flow under load. Run it
and include screenshots showing your metrics and traces responding to the load in Grafana.
The load test does not need to be sophisticated — it needs to generate enough signal to make
your dashboard meaningful.
5. Critique
Write a short section (half a page to one page) addressing:
- What in nopCommerce's design helped or hindered your instrumentation work?
- If you were making architectural decisions on this project going forward, what would you
change to make it more observable — and at what cost?
- Where did you have to make a surgical change to the existing code? Why was it
necessary, and how did you minimise the impact?
This is the architect's voice. It matters as much as the code.