#!/bin/bash

PROJECT_MAIN="main" # Archivo principal del proyecto
REPORT_TEX="Moogle-latex.tex" # Archivo tex del informe
SLIDES_TEX="Moogle-latex-presentation.tex" # Archivo tex de las diapositivas
REPORT_PDF="Moogle-latex.pdf" # Archivo pdf del informe
SLIDES_PDF="Moogle-latex-presentation.pdf" # Archivo pdf de las diapositivas



function clean {
    cd  "../informe"
    rm -f *.log *.aux *.out
    cd "../presentacion"
    rm -f *.log *.aux *.out *.toc *.snm *.nav
    
    
}

function run_project {
    cd ".."
    
    echo "Ejecutando el proyecto..."
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
    
    if [[ "$OSTYPE" == "darwin"* ]]; then
        open $REPORT_PDF
    elif [[ "$OSTYPE" == "msys" ]]; then
        start $REPORT_PDF
    else
        echo "Unsupported operating system: $OSTYPE"
        exit 1
    fi
}

function show_slides {
    cd "../presentacion"
    if [ ! -f "$SLIDES_PDF" ]; then
        compile_slides
    fi
    
    if [[ "$OSTYPE" == "darwin"* ]]; then
        open $SLIDES_PDF
    elif [[ "$OSTYPE" == "msys" ]]; then
        start $SLIDES_PDF
    else
        echo "Unsupported operating system: $OSTYPE"
        exit 1
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
        show_report $2
        ;;
    show_slides)
        show_slides $2
        ;;
    *)
        echo "Uso: $0 {run|clean|report|slides|show_report|show_slides}"
        exit 1
esac

exit 0
