#!/bin/bash

PROJECT_MAIN="main" # Archivo principal del proyecto
REPORT_TEX="Moogle-latex.tex" # Archivo tex del informe
SLIDES_TEX="Moogle-latex-presentation.tex" # Archivo tex de las diapositivas
REPORT_PDF="Moogle-latex.pdf" # Archivo pdf del informe
SLIDES_PDF="Moogle-latex-presentation.pdf" # Archivo pdf de las diapositivas

# La carpeta contentWithSpaces y los json creados por mi para guardar objetos de mi proyecto no son eliminados pues forman parte del proyecto en si.
function clean {
    cd  "../informe"
    rm -f *.log *.aux *.out *.pdf
    cd "../presentacion"
    rm -f *.log *.aux *.out *.toc *.snm *.nav *.pdf
    
    cd ".."
    find . -type d -name bin -exec rm -rf {} +
    find . -type d -name obj -exec rm -rf {} +
    find . -type d -name .vs -exec rm -rf {} +

    echo "Proyecto limpiado."
}

function run_project {
    cd ".."
    
    make dev

}

function compile_report {
    cd "../informe"
    pdflatex $REPORT_TEX
}

function compile_slides {
    cd "../presentacion"
    pdflatex $SLIDES_TEX
}

function show_report {

    cd "../informe"
    if [ ! -f "$REPORT_PDF" ]; then
        compile_report
    fi
    
    if [[ "$1" == "-v" ]]; then
        if [[ ! "$2" == "" ]]; then
            echo "Usando visualizador alternativo $2"
            $2 $REPORT_PDF
        else
            echo "Opcion de parametro -v no valida"
        fi
    else
  
        if [[ "$OSTYPE" == "linux-gnu"* ]]; then
            xdg-open $REPORT_PDF
        elif [[ "$OSTYPE" == "darwin"* ]]; then
            open $REPORT_PDF
        else
            start $REPORT_PDF
        fi
    fi
}

function show_slides {
    cd "../presentacion"
    if [ ! -f "$SLIDES_PDF" ]; then
        compile_slides
    fi
    
    if [[ "$1" == "-v" ]]; then
        if [[ ! "$2" == "" ]]; then
            echo "Usando visualizador alternativo $2"
            $2 $SLIDES_PDF
        else
            echo "Opcion de parametro -v no valida"
        fi
    else
  
        if [[ "$OSTYPE" == "linux-gnu"* ]]; then
            xdg-open $SLIDES_PDF
        elif [[ "$OSTYPE" == "darwin"* ]]; then
            open $SLIDES_PDF
        else
            start $SLIDES_PDF
        fi
    fi
}

case $1 in
    run)
        run_project
        ;;
    clean)
        clean
        ;;
    report)
        compile_report
        ;;
    slides)
        compile_slides
        ;;
    show_report)
        show_report $2 $3
        ;;
    show_slides)
        show_slides $2 $3
        ;;
    *)
        echo "Uso: $0 {run|clean|report|slides|show_report|show_report -v option |show_slides|show_slides -v option|}"
        exit 1
esac

exit 0
