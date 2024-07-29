pipeline {
    agent any

    environment {
        APP_NAME = "realestate"
        DOCKER_IMAGE_NAME = "${env.APP_NAME}"
        DOCKER_COMPOSE_PATH = "~/docker-compose.yml"
    }

    stages {
        stage('Build') {
            steps {
                script {
                    // Build the Docker image
                    docker.build(env.DOCKER_IMAGE_NAME)
                }
            }
        }
        stage('Deploy') {
            steps {
                script {
                    // Directly load the Docker image (no need for compression)
                    sh """
                    docker save ${env.DOCKER_IMAGE_NAME} | docker load
                    """

                    // Move docker-compose.yml to the correct location
                    sh """
                    cp docker-compose.yml ${env.DOCKER_COMPOSE_PATH}
                    """

                    // Deploy using Docker Compose
                    sh """
                    export COMPOSE_PROJECT_NAME=${env.APP_NAME}
                    docker-compose -f ${env.DOCKER_COMPOSE_PATH} down
                    docker-compose -f ${env.DOCKER_COMPOSE_PATH} up -d --build
                    """
                }
            }
        }

        
    }

    post {
        always {
            cleanWs()
        }
    }
}
