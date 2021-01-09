# Installing the doc generation tools

## Install Python

This is currently using Python 3.9. You can get it here: https://www.python.org/downloads/

## Install Sphinx

From a command/PowerShell prompt:
* `pip install sphinx`
* `python -m pip install --upgrade pip`

## Install the theme

* `pip install sphinx-rtd-theme`


# Building the documentation

Navigate to the `docs` folder and run:
```powershell
./make html
```

## Running a HTTP server

In a separate terminal, navigate to the newly created `docs/_build/html` folder, and run:
```powershell
python -m http.server 8000
```

## More information

For more information see: https://www.sphinx-doc.org/en/1.5.1/tutorial.html