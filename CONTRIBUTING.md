# Contributing to the Barcode Attendance System

We welcome contributions from the community! If you'd like to contribute to the Barcode Attendance System, please follow these guidelines.

## How to Contribute

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes.
4.  Submit a pull request.

## Getting Started

To get started, you'll need to have Node.js and npm installed on your machine.

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/BAS-Web.git
    cd BAS-Web
    ```

2.  **Navigate to the project directory:**
    The Vue.js project is located in the `BAS` sub-directory.
    ```bash
    cd BAS
    ```

3.  **Install dependencies:**
    ```bash
    npm install
    ```

4.  **Set up environment variables:**
    Create a `.env` file in the `BAS` directory by copying the `.env.example` file. Then, fill in the required Supabase URL and anonymous key.
    ```bash
    cp .env.example .env
    ```

5.  **Run the development server:**
    ```bash
    npm run dev
    ```

The application will be available at the local address provided by Vite (e.g., `http://localhost:5173`).

## Branching Strategy

We use a simple branching strategy:

*   `main`: The main branch. All pull requests should be made against this branch.
*   `feature/*`: Feature branches. Create a new feature branch for each new feature you work on.
*   `bugfix/*`: Bug fix branches. Create a new bug fix branch for each bug you fix.

## Pull Request Process

1.  Ensure that your code lints and builds without errors.
2.  Include a clear and descriptive title for your pull request.
3.  Provide a detailed description of the changes you've made.
4.  If your pull request addresses an existing issue, please reference the issue number in the description.

## Coding Style

We use ESLint to enforce a consistent coding style. Before submitting a pull request, please make sure that your code lints without any errors.

To lint your code, run the following command in the `BAS` directory:

```bash
npm run lint
```
