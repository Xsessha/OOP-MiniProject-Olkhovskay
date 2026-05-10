```mermaid
flowchart TD

A[Push or Pull Request] --> B[Checkout Repository]

B --> C[Restore Dependencies]
C --> D[Build Solution]

D --> E[Run Unit Tests]
E --> F[Run Integration Tests]

F --> G[Collect Code Coverage]

G --> H[Generate Coverage Report]

H --> I{Quality Gate}

I -->|Coverage >= 85 and Tests Pass| J[CI Passed]
I -->|Tests Failed| K[CI Failed]
I -->|Coverage Below Threshold| K[CI Failed]

J --> L[Publish Artifacts]