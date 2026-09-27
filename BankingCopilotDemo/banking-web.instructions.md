---
description: Rules for the banking browser page
applyTo: "**/*.html"
---

- Keep the browser page usable without a JavaScript framework.
- Use semantic HTML and labels connected to form controls.
- Show a visible success or error message after each operation.
- Put stable `id` attributes on interactive elements for Selenium tests.
- Insert server values using `textContent`, not `innerHTML`.